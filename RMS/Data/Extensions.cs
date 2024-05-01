using System.Security.Claims;

namespace RMS.Data
{
    public static class Extensions
    { 
        public static string? GetUserId(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
