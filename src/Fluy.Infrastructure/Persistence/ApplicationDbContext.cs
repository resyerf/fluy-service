using System.Reflection;
using Fluy.Application.Common.Interfaces;
using Fluy.Domain.Common;
using Fluy.Domain.Approvals;
using Fluy.Domain.Identity;
using Fluy.Domain.Notifications;
using Fluy.Domain.Requests;
using Fluy.Domain.Rules;
using Fluy.Domain.Tenancy;
using Fluy.Domain.Workflows;
using Fluy.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace Fluy.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentTenantService currentTenant)
    : DbContext(options), IUnitOfWork
{
    private static readonly MethodInfo SetTenantQueryFilterMethod = typeof(ApplicationDbContext)
        .GetMethod(nameof(SetTenantQueryFilter), BindingFlags.NonPublic | BindingFlags.Instance)!;

    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<Department> Departments => Set<Department>();

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<PasswordSetToken> PasswordSetTokens => Set<PasswordSetToken>();

    public DbSet<Request> Requests => Set<Request>();
    public DbSet<RequestField> RequestFields => Set<RequestField>();
    public DbSet<Approval> Approvals => Set<Approval>();
    public DbSet<ApprovalRule> ApprovalRules => Set<ApprovalRule>();

    public DbSet<WorkflowDefinition> WorkflowDefinitions => Set<WorkflowDefinition>();
    public DbSet<WorkflowVersion> WorkflowVersions => Set<WorkflowVersion>();
    public DbSet<WorkflowStep> WorkflowSteps => Set<WorkflowStep>();
    public DbSet<WorkflowTransition> WorkflowTransitions => Set<WorkflowTransition>();
    public DbSet<WorkflowInstance> WorkflowInstances => Set<WorkflowInstance>();

    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Schema explícito (CODE.md §10-D15): "tenant" para simetría con el schema "platform" de
        // fluy-admin-service, que comparte la misma instancia de Postgres (CODE.md §9.4).
        modelBuilder.HasDefaultSchema("tenant");

        // AggregateRoot.DomainEvents no es un dato persistente (se despacha y se limpia en el mismo
        // ciclo de SaveChanges) — se ignora explícitamente para que EF Core no intente mapearlo.
        modelBuilder.Ignore<DomainEvent>();

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Global Query Filter multi-tenant (CODE.md §4.8). El filtro se expresa como lambda C#
        // real (no como Expression.Constant construido a mano) para que EF Core re-apunte "this"
        // a cada instancia de ApplicationDbContext en tiempo de ejecución, en vez de congelar la
        // instancia de ICurrentTenantService vigente cuando se compiló el modelo la primera vez.
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(ITenantEntity).IsAssignableFrom(entityType.ClrType))
            {
                continue;
            }

            SetTenantQueryFilterMethod.MakeGenericMethod(entityType.ClrType).Invoke(this, [modelBuilder]);
        }

        base.OnModelCreating(modelBuilder);
    }

    private void SetTenantQueryFilter<TEntity>(ModelBuilder modelBuilder) where TEntity : class, ITenantEntity
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(e => e.TenantId == currentTenant.TenantId);
    }
}
