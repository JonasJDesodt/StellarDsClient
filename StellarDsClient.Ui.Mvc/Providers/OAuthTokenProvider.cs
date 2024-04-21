using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;
using StellarDsClient.Sdk;
using StellarDsClient.Sdk.Abstractions;
using StellarDsClient.Ui.Mvc.Stores;
using System.Security.Claims;
using System.Security;
using StellarDsClient.Sdk.Dto.Transfer;
using StellarDsClient.Ui.Mvc.Extensions;

namespace StellarDsClient.Ui.Mvc.Providers
{
    //todo: rename, it does more than only providing a token
    public class OAuthTokenProvider(OAuthTokenStore oAuthTokenStore, OAuthApiService oAuthApiService, IHttpContextAccessor httpContextAccessor) : ITokenProvider
    {
        public OAuthApiService OAuthApiService => oAuthApiService;

        public async Task<string> Get()
        {
            var accessToken = oAuthTokenStore.GetAccessToken();

            if (accessToken is not null && ValidateAccessToken(accessToken))
            {
                return accessToken;
            }

            var tokens = await oAuthApiService.PostRefreshTokenAsync(oAuthTokenStore.GetRefreshToken());

            await BrowserSignIn(tokens);

            return tokens.AccessToken;
        }

        public static bool ValidateAccessToken(string accessToken)
        {
            return new JsonWebTokenHandler().ReadJsonWebToken(accessToken).ValidTo > DateTime.UtcNow;
        }

        //todo: rename
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

            //oAuthTokenStore.SaveAccessToken(oAuthTokens.AccessToken, new DateTimeOffset(accessJsonWebToken.ValidTo));
            oAuthTokenStore.SaveAccessToken(oAuthTokens.AccessToken, new DateTimeOffset(DateTime.Now.AddSeconds(15)));


            var refreshJsonWebToken = handler.ReadJsonWebToken(oAuthTokens.RefreshToken) ?? throw new SecurityException("Token could not be converted");

            var refreshJsonWebTokenExpiry = new DateTimeOffset(refreshJsonWebToken.ValidTo);

            await httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), new AuthenticationProperties() { ExpiresUtc = refreshJsonWebTokenExpiry });

            oAuthTokenStore.SaveRefreshToken(oAuthTokens.RefreshToken, refreshJsonWebTokenExpiry);
        }

        //todo: rename
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