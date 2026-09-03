using Fluy.Application.Interfaces.Services;
using Fluy.Application.Interfaces.Repositories;
using Fluy.Infrastructure.Identity.Services;
using Fluy.Infrastructure.External.Services;
using Fluy.Infrastructure.Persistence;
using Fluy.Infrastructure.Persistence.Context;
using Fluy.Infrastructure.Persistence.Interceptors;
using Fluy.Infrastructure.Persistence.Repositories;
using Fluy.SharedKernel;
using Fluy.SharedKernel.Dispatching;
using Fluy.SharedKernel.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fluy.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<AuditableEntitiesInterceptor>();
        services.AddScoped<TenantIntegrityInterceptor>();

        var connectionString = configuration.GetConnectionString("ApplicationDb");

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            // Historial de migraciones explícito en el schema "tenant" (CODE.md §10-D15): al
            // compartir la misma instancia de Postgres con fluy-admin-service (schema "platform"),
            // cada servicio necesita su propia tabla __EFMigrationsHistory para no chocar.
            options.UseNpgsql(connectionString, npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "tenant"));
            options.AddInterceptors(
                sp.GetRequiredService<AuditableEntitiesInterceptor>(),
                sp.GetRequiredService<TenantIntegrityInterceptor>());
        });

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<ApplicationDbContextInitializer>();

        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<IBranchRepository, BranchRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IRequestRepository, RequestRepository>();
        services.AddScoped<IApprovalRepository, ApprovalRepository>();
        services.AddScoped<IApprovalRuleRepository, ApprovalRuleRepository>();
        services.AddScoped<IWorkflowInstanceRepository, WorkflowInstanceRepository>();
        services.AddScoped<IWorkflowVersionRepository, WorkflowVersionRepository>();
        services.AddScoped<IWorkflowDefinitionRepository, WorkflowDefinitionRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();

        // Lectura de solo consulta del schema "platform" (CODE.md §9.4) — nunca gestiona migraciones.
        services.AddDbContext<PlatformReadDbContext>(options => options.UseNpgsql(connectionString));
        services.AddMemoryCache();
        services.AddScoped<ITenantDirectory, TenantDirectory>();
        services.AddScoped<IEntitlementReader, EntitlementReader>();

        services.AddScoped<ICurrentTenantService, CurrentTenantService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IPermissionChecker, PermissionChecker>();
        services.AddSingleton<IDateTime, SystemDateTime>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        services.AddScoped<IEmailSender, SmtpEmailSender>();
        services.AddScoped<IFrontendLinkBuilder, FrontendLinkBuilder>();
        services.Configure<SmtpSettings>(configuration.GetSection(SmtpSettings.SectionName));
        services.Configure<FrontendSettings>(configuration.GetSection(FrontendSettings.SectionName));

        return services;
    }
}
