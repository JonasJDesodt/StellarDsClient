using System.Security.Claims;

namespace StellarDsClient.Ui.Mvc.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string? GetId(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        }

        public static string? GetName(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        }
    }
}
