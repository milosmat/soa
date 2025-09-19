using System.Security.Claims;

namespace PurchaseApi.Web.Auth
{
    public static class UserCtx
    {
        public static string UserId(this ClaimsPrincipal u) =>
            u.FindFirstValue(ClaimTypes.NameIdentifier) ?? u.FindFirstValue("sub")!;
    }
}
