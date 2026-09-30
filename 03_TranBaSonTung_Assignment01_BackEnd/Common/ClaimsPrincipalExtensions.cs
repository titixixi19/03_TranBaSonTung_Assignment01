using System.Security.Claims;

namespace BackEnd.Common;

public static class ClaimsPrincipalExtensions
{
    public static short GetAccountId(this ClaimsPrincipal user) =>
        short.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : (short)0;
}
