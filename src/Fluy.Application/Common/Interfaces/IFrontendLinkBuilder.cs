namespace Fluy.Application.Common.Interfaces;

/// <summary>
/// Construye URLs hacia fluy-web para los links que van en emails (CODE.md §4.17). Hoy un único
/// `Frontend:BaseUrl` de desarrollo (`http://localhost:4200`) — sin subdominios reales `*.fluy.com`
/// todavía (CLAUDE.md §41), el subdominio del tenant viaja como query param.
/// </summary>
public interface IFrontendLinkBuilder
{
    string BuildSetPasswordLink(string tenantSubdomain, string rawToken);
}
