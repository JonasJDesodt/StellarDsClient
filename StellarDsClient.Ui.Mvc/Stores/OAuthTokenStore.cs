using System.Runtime.CompilerServices;
using StellarDsClient.Ui.Mvc.Models.Settings;

namespace StellarDsClient.Ui.Mvc.Stores
{
    public class OAuthTokenStore(IHttpContextAccessor httpContextAccessor, CookieSettings cookieSettings) 
    {
        private string? _accessTokenScopedStore;
        private string? _refreshTokenScopedStore;

        public string? GetAccessToken()
        {
            return _accessTokenScopedStore ?? GetToken(nameof(CookieSettings.OAuthCookies.AccessToken));
        }

        /// <summary>
        /// Throws a NullReferenceException if the token has expired and the user is not signed out from the HttpContext.
        /// This method should not be called when the user is not signed in.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public string GetRefreshToken()
        {
            return _refreshTokenScopedStore ?? GetToken(nameof(CookieSettings.OAuthCookies.RefreshToken)) ?? throw new NullReferenceException(nameof(CookieSettings.OAuthCookies.RefreshToken));
        }

        public void SaveAccessToken(string token, DateTimeOffset expires)
        {
            cookieSettings.OAuthCookies.AccessToken.Expires = expires; // todo: check if max-age is available from api, test if the options are reset when the service is injected

            SaveToken(nameof(CookieSettings.OAuthCookies.AccessToken), token, cookieSettings.OAuthCookies.AccessToken);

            _accessTokenScopedStore = token;
        }

        public void SaveRefreshToken(string token, DateTimeOffset expires)
        {
            cookieSettings.OAuthCookies.RefreshToken.Expires = expires;

            SaveToken(nameof(CookieSettings.OAuthCookies.RefreshToken), token, cookieSettings.OAuthCookies.RefreshToken);

            _refreshTokenScopedStore = token;
        }

        private void SaveToken(string key, string token, CookieOptions options)
        {
            ClearToken(key);

            httpContextAccessor.HttpContext?.Response.Cookies.Append(key, token, options);
        }

        private string? GetToken(string key)
        {
            string? jwtToken = null;

            httpContextAccessor.HttpContext?.Request.Cookies.ContainsKey(key);

            httpContextAccessor.HttpContext?.Request.Cookies.TryGetValue(key, out jwtToken);

            return jwtToken;
        }

        private void ClearToken(string key)
        {
            httpContextAccessor.HttpContext?.Response.Cookies.Delete(key);
        }
    }
}