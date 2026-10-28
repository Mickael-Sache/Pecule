using System.Security.Claims;

using Microsoft.AspNetCore.Http;

namespace Pecule.Accounts;

internal sealed class CurrentUser(IHttpContextAccessor httpContextAccessor)
{
    public Guid? Id =>
        Guid.TryParse(httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier), out var id)
            ? id
            : null;
}
