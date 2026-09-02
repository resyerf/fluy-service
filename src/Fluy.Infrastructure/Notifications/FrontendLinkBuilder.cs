using Fluy.Application.Common.Interfaces;
using Microsoft.Extensions.Options;

namespace Fluy.Infrastructure.Notifications;

public class FrontendLinkBuilder(IOptions<FrontendSettings> options) : IFrontendLinkBuilder
{
    public string BuildSetPasswordLink(string tenantSubdomain, string rawToken) =>
        $"{options.Value.BaseUrl}/set-password?tenant={Uri.EscapeDataString(tenantSubdomain)}&token={Uri.EscapeDataString(rawToken)}";
}
