using Fluy.Application.DTOs;
using Fluy.Application.Interfaces.Repositories;
using Fluy.Application.Interfaces.Services;
using Fluy.Domain.Entities;
using Fluy.Domain.Enums;
using Fluy.SharedKernel;
using Fluy.SharedKernel.Dispatching;
using Fluy.SharedKernel.Security;

namespace Fluy.Application.Commands.Identity.SeedDemoTenantData;

/// <summary>
/// Puebla el tenant demo (subdominio "demo") con organización, roles, usuarios adicionales, un
/// workflow publicado con transiciones condicionales (CLAUDE.md §14-16, el mismo ejemplo "¿Monto
/// > 50000?" de CLAUDE.md §3) y solicitudes de ejemplo que cubren todo el ciclo de vida. Se invoca
/// una sola vez, justo después de BootstrapTenant, desde DemoTenantSeeder (fluy-admin-service) al
/// arrancar en Development. Reconstruye a mano la misma secuencia de efectos que ya producen
/// SubmitRequestCommandHandler/ApproveRequestCommandHandler/RejectRequestCommandHandler/
/// RequestCorrectionCommandHandler — no los reusa directamente porque esos dependen de
/// ICurrentUserService (quién está logueado), y acá el "actor" de cada decisión varía por escenario.
/// Idempotente: si ya existe una Company para el tenant, no hace nada.
/// </summary>
public class SeedDemoTenantDataCommandHandler(
    ICompanyRepository companies,
    IBranchRepository branches,
    IDepartmentRepository departments,
    IUserRepository users,
    IRoleRepository roles,
    IUserRoleRepository userRoles,
    IPermissionRepository permissions,
    IWorkflowDefinitionRepository workflowDefinitions,
    IWorkflowVersionRepository workflowVersions,
    IWorkflowInstanceRepository workflowInstances,
    IRequestRepository requests,
    IApprovalRepository approvals,
    INotificationRepository notifications,
    IUnitOfWork unitOfWork,
    ICurrentTenantService currentTenant,
    IPasswordHasher passwordHasher,
    IDateTime dateTime)
    : ICommandHandler<SeedDemoTenantDataCommand, SeedDemoTenantDataResult>
{
    private const string DemoPassword = "clavedemo123";
    private const string MasterEmail = "usuario@demo.com";

    private sealed record ApproverAssignment(User User, Guid RoleId);

    public async Task<SeedDemoTenantDataResult> Handle(SeedDemoTenantDataCommand command, CancellationToken cancellationToken)
    {
        var tenantId = currentTenant.TenantId!.Value;

        var alreadySeeded = (await companies.GetAllAsync(cancellationToken)).Count > 0;
        if (alreadySeeded)
        {
            return new SeedDemoTenantDataResult(Seeded: false);
        }

        var masterUser = await users.GetByEmailAsync(MasterEmail, cancellationToken)
            ?? throw new InvalidOperationException(
                $"SeedDemoTenantDataCommand requiere que '{MasterEmail}' ya exista (BootstrapTenant debe ejecutarse antes).");
        masterUser.ChangePassword(passwordHasher.Hash(DemoPassword));

        var limaBranch = SeedOrganization(tenantId);
        var (gerente, cfo) = await SeedApproversAsync(tenantId, cancellationToken);

        var now = dateTime.UtcNow;
        var (definition, version, gerenteStep, cfoStep) = SeedWorkflow(tenantId, gerente.RoleId, cfo.RoleId, now);

        SeedRequests(tenantId, masterUser, limaBranch.Id, gerente, cfo, definition, version, gerenteStep, cfoStep, now);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new SeedDemoTenantDataResult(Seeded: true);
    }

    private Branch SeedOrganization(Guid tenantId)
    {
        var company = Company.Create(tenantId, "Empresa Demo S.A.", legalIdentifier: "20123456789");
        companies.Add(company);

        var lima = Branch.Create(tenantId, company.Id, "Lima");
        var arequipa = Branch.Create(tenantId, company.Id, "Arequipa");
        branches.Add(lima);
        branches.Add(arequipa);

        departments.Add(Department.Create(tenantId, lima.Id, "Finanzas"));
        departments.Add(Department.Create(tenantId, lima.Id, "Compras"));

        return lima;
    }

    private async Task<(ApproverAssignment Gerente, ApproverAssignment Cfo)> SeedApproversAsync(
        Guid tenantId, CancellationToken cancellationToken)
    {
        var approvalPermissions = await permissions.GetByCodesAsync(
            ["request.view", "request.approve", "request.reject"], cancellationToken);

        var gerenteRole = Role.Create(tenantId, "Gerente");
        var cfoRole = Role.Create(tenantId, "CFO");
        roles.Add(gerenteRole);
        roles.Add(cfoRole);
        roles.AddPermissions(approvalPermissions.SelectMany(p => new[]
        {
            RolePermission.Create(tenantId, gerenteRole.Id, p.Id),
            RolePermission.Create(tenantId, cfoRole.Id, p.Id)
        }));

        var gerenteUser = User.Create(tenantId, "gerente@demo.com", "Gerente Demo", passwordHasher.Hash(DemoPassword));
        var cfoUser = User.Create(tenantId, "cfo@demo.com", "CFO Demo", passwordHasher.Hash(DemoPassword));
        users.Add(gerenteUser);
        users.Add(cfoUser);

        userRoles.Add(UserRole.Create(tenantId, gerenteUser.Id, gerenteRole.Id));
        userRoles.Add(UserRole.Create(tenantId, cfoUser.Id, cfoRole.Id));

        return (new ApproverAssignment(gerenteUser, gerenteRole.Id), new ApproverAssignment(cfoUser, cfoRole.Id));
    }

    /// <summary>
    /// Reproduce el ejemplo de CLAUDE.md §3/§16: Gerente aprueba montos hasta 50.000; por encima de
    /// ese monto escala a CFO. Ambos pasos terminan la solicitud si no hay condición que los desvíe.
    /// </summary>
    private (WorkflowDefinition Definition, WorkflowVersion Version, WorkflowStep GerenteStep, WorkflowStep CfoStep) SeedWorkflow(
        Guid tenantId, Guid gerenteRoleId, Guid cfoRoleId, DateTimeOffset now)
    {
        var definition = WorkflowDefinition.Create(
            tenantId, "Solicitud de Pago", "Flujo estándar de aprobación de solicitudes de pago.");
        workflowDefinitions.Add(definition);

        var version = WorkflowVersion.CreateDraft(tenantId, definition.Id, versionNumber: 1);
        workflowVersions.Add(version);

        var gerenteStep = WorkflowStep.Create(tenantId, version.Id, "Aprobación Gerente", gerenteRoleId, order: 1);
        var cfoStep = WorkflowStep.Create(tenantId, version.Id, "Aprobación CFO", cfoRoleId, order: 2);
        workflowVersions.AddStep(gerenteStep);
        workflowVersions.AddStep(cfoStep);

        version.SetInitialStep(gerenteStep.Id);

        var escalateToCfo = WorkflowTransition.Create(
            tenantId, version.Id, gerenteStep.Id, cfoStep.Id,
            "Amount", WorkflowConditionOperator.GreaterThan, 50000m, order: 1);
        var gerenteCompletes = WorkflowTransition.Create(
            tenantId, version.Id, gerenteStep.Id, null, null, null, null, order: 2);
        var cfoCompletes = WorkflowTransition.Create(
            tenantId, version.Id, cfoStep.Id, null, null, null, null, order: 1);

        workflowVersions.AddTransition(escalateToCfo);
        workflowVersions.AddTransition(gerenteCompletes);
        workflowVersions.AddTransition(cfoCompletes);

        version.Publish([gerenteStep, cfoStep], [escalateToCfo, gerenteCompletes, cfoCompletes], now);
        definition.MarkPublished();

        return (definition, version, gerenteStep, cfoStep);
    }

    private void SeedRequests(
        Guid tenantId, User requester, Guid branchId, ApproverAssignment gerente, ApproverAssignment cfo,
        WorkflowDefinition definition, WorkflowVersion version, WorkflowStep gerenteStep, WorkflowStep cfoStep,
        DateTimeOffset now)
    {
        // 1. Draft — nunca enviada.
        requests.Add(Request.Create(
            tenantId, requester.Id, "Compra de laptops", "Dos laptops para el equipo de desarrollo.", 3500m, branchId));

        // 2. Submitted — pendiente de Gerente.
        var (req2, _, _) = StartRequest(
            tenantId, requester.Id, branchId, "Reembolso de viáticos", "Viáticos del viaje a Arequipa.", 1200m,
            definition, version, gerenteStep, gerente.RoleId, now);
        NotifyApprovalAssigned(tenantId, gerente.User.Id, requester.Id, req2);

        // 3. Submitted — Gerente ya aprobó, pendiente de CFO (monto escala por condición).
        var (req3, instance3, approval3) = StartRequest(
            tenantId, requester.Id, branchId, "Compra de servidor", "Servidor para el datacenter de Lima.", 80000m,
            definition, version, gerenteStep, gerente.RoleId, now);
        approval3.Approve(gerente.User.Id, "Aprobado, monto justificado.", now);
        instance3.MoveTo(cfoStep.Id);
        var approval3Tier2 = Approval.CreatePending(
            tenantId, req3.Id, tier: 2, requiredRoleId: cfo.RoleId,
            workflowInstanceId: instance3.Id, workflowStepId: cfoStep.Id);
        approvals.Add(approval3Tier2);
        NotifyApprovalAssigned(tenantId, cfo.User.Id, gerente.User.Id, req3);

        // 4. Completed — un solo paso (Gerente).
        var (req4, instance4, approval4) = StartRequest(
            tenantId, requester.Id, branchId, "Pago de proveedor local", "Pago de factura a proveedor local.", 2000m,
            definition, version, gerenteStep, gerente.RoleId, now);
        approval4.Approve(gerente.User.Id, "Aprobado.", now);
        instance4.Complete(now);
        req4.Complete();
        NotifyRequestApproved(tenantId, requester.Id, gerente.User.Id, req4);

        // 5. Completed — dos pasos (Gerente + CFO).
        var (req5, instance5, approval5) = StartRequest(
            tenantId, requester.Id, branchId, "Compra de equipo de cómputo mayor",
            "Renovación de equipos de cómputo para toda la sede.", 120000m,
            definition, version, gerenteStep, gerente.RoleId, now);
        approval5.Approve(gerente.User.Id, "Escalado a CFO por el monto.", now);
        instance5.MoveTo(cfoStep.Id);
        var approval5Tier2 = Approval.CreatePending(
            tenantId, req5.Id, tier: 2, requiredRoleId: cfo.RoleId,
            workflowInstanceId: instance5.Id, workflowStepId: cfoStep.Id);
        approvals.Add(approval5Tier2);
        approval5Tier2.Approve(cfo.User.Id, "Aprobado por CFO.", now);
        instance5.Complete(now);
        req5.Complete();
        NotifyRequestApproved(tenantId, requester.Id, cfo.User.Id, req5);

        // 6. Rejected por Gerente.
        var (req6, instance6, approval6) = StartRequest(
            tenantId, requester.Id, branchId, "Solicitud de anticipo", "Anticipo de sueldo.", 5000m,
            definition, version, gerenteStep, gerente.RoleId, now);
        const string rejectionComment = "Monto no justificado.";
        approval6.Reject(gerente.User.Id, rejectionComment, now);
        req6.Reject();
        instance6.Cancel();
        notifications.Add(Notification.Create(
            tenantId, requester.Id, NotificationType.RequestRejected,
            title: "Tu solicitud fue rechazada", message: rejectionComment,
            actorUserId: gerente.User.Id, requestId: req6.Id));

        // 7. Devuelta para corrección por Gerente.
        var (req7, _, approval7) = StartRequest(
            tenantId, requester.Id, branchId, "Compra de insumos de oficina", "Insumos de oficina para Lima.", 800m,
            definition, version, gerenteStep, gerente.RoleId, now);
        const string correctionComment = "Falta adjuntar cotización.";
        approval7.ReturnForCorrection(gerente.User.Id, correctionComment, now);
        req7.ReturnForCorrection();
        notifications.Add(Notification.Create(
            tenantId, requester.Id, NotificationType.RequestReturnedForCorrection,
            title: "Tu solicitud requiere corrección", message: correctionComment,
            actorUserId: gerente.User.Id, requestId: req7.Id));
    }

    private (Request Request, WorkflowInstance Instance, Approval Approval) StartRequest(
        Guid tenantId, Guid requesterId, Guid branchId, string title, string description, decimal amount,
        WorkflowDefinition definition, WorkflowVersion version, WorkflowStep initialStep, Guid initialStepRoleId,
        DateTimeOffset now)
    {
        var request = Request.Create(tenantId, requesterId, title, description, amount, branchId);
        request.Submit(now);
        requests.Add(request);

        var instance = WorkflowInstance.Start(tenantId, definition.Id, version.Id, request.Id, initialStep.Id);
        workflowInstances.Add(instance);

        var approval = Approval.CreatePending(
            tenantId, request.Id, tier: 1, requiredRoleId: initialStepRoleId,
            workflowInstanceId: instance.Id, workflowStepId: initialStep.Id);
        approvals.Add(approval);

        return (request, instance, approval);
    }

    private void NotifyApprovalAssigned(Guid tenantId, Guid recipientUserId, Guid actorUserId, Request request) =>
        notifications.Add(Notification.Create(
            tenantId, recipientUserId, NotificationType.ApprovalAssigned,
            title: "Nueva solicitud pendiente de tu aprobación", message: request.Title,
            actorUserId: actorUserId, requestId: request.Id));

    private void NotifyRequestApproved(Guid tenantId, Guid recipientUserId, Guid actorUserId, Request request) =>
        notifications.Add(Notification.Create(
            tenantId, recipientUserId, NotificationType.RequestApproved,
            title: "Tu solicitud fue aprobada", message: request.Title,
            actorUserId: actorUserId, requestId: request.Id));
}
