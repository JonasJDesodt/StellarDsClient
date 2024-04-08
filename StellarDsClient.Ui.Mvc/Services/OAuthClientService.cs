using System.Security;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.JsonWebTokens;
using StellarDsClient.Dto.Transfer;
using StellarDsClient.Sdk.Abstractions;
using StellarDsClient.Ui.Mvc.Extensions;

namespace StellarDsClient.Ui.Mvc.Services
{
    public class OAuthClientService(IOAuthTokenStore oAuthTokenStore, IHttpContextAccessor httpContextAccessor) : IOAuthClientService
    {
        public async Task BrowserSignIn(OAuthTokens oAuthTokens)
        {
            if (httpContextAccessor.HttpContext is null)
            {
                return;
            }


            var handler = new JsonWebTokenHandler();

            var accessJsonWebToken = handler.ReadJsonWebToken(oAuthTokens.AccessToken) ?? throw new SecurityException("Token could not be converted");

            var claims = accessJsonWebToken.Claims.ToList();

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            oAuthTokenStore.SaveAccessToken(oAuthTokens.AccessToken, new DateTimeOffset(accessJsonWebToken.ValidTo));


            var refreshJsonWebToken = handler.ReadJsonWebToken(oAuthTokens.RefreshToken) ?? throw new SecurityException("Token could not be converted");

            var refreshJsonWebTokenExpiry = new DateTimeOffset(refreshJsonWebToken.ValidTo);
 
            await httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), new AuthenticationProperties() { ExpiresUtc = refreshJsonWebTokenExpiry });
            
            oAuthTokenStore.SaveRefreshToken(oAuthTokens.RefreshToken, refreshJsonWebTokenExpiry);
        }

        public async Task BrowserSignOut()
        {
            if (httpContextAccessor.HttpContext is null)
            {
                return;
            }

            await httpContextAccessor.HttpContext.ClearApplicationCookies().SignOutAsync();
        }
    }
}
