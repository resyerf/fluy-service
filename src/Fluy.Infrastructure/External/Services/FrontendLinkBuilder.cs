using Fluy.Application.Interfaces.Services;
using Microsoft.Extensions.Options;

namespace Fluy.Infrastructure.External.Services;

public class FrontendLinkBuilder(IOptions<FrontendSettings> options) : IFrontendLinkBuilder
{
    public string BuildSetPasswordLink(string tenantSubdomain, string rawToken) =>
        $"{options.Value.BaseUrl}/set-password?tenant={Uri.EscapeDataString(tenantSubdomain)}&token={Uri.EscapeDataString(rawToken)}";
}
