using System.Reflection;
using Fluy.Application.Services;
using Fluy.Application.Interfaces.Services;
using Fluy.SharedKernel.Dispatching;
using Microsoft.Extensions.DependencyInjection;

namespace Fluy.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddDispatcher(Assembly.GetExecutingAssembly());

        services.AddScoped<IApprovalAuthorizationService, ApprovalAuthorizationService>();
        services.AddScoped<INotificationRecipientResolver, NotificationRecipientResolver>();

        return services;
    }
}
