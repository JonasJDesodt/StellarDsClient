using StellarDsClient.Sdk.Exceptions;
using System.Runtime.CompilerServices;

namespace StellarDsClient.Ui.Mvc.Stores
{
    public class OAuthTokenStore(IHttpContextAccessor httpContextAccessor)
    {
        private string? _accessTokenScopedStore;
        private string? _refreshTokenScopedStore;

        public string? GetAccessToken()
        {
            return _accessTokenScopedStore ?? GetToken("AccessToken");
        }

        /// <summary>
        /// Throws a NullReferenceException if the token has expired and the user is not signed out from the HttpContext.
        /// This method should not be called when the user is not signed in.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public string GetRefreshToken()
        {
            return _refreshTokenScopedStore ?? GetToken("RefreshToken") ?? throw new CustomUnauthorizedException("Unauthorized", new NullReferenceException("The RefreshToken cookie returned null"));
        }

        public void SaveAccessToken(string token, DateTimeOffset expires)
        {

            SaveToken("AccessToken", token, new CookieOptions 
            {
                IsEssential = true,
                Expires = expires,
                HttpOnly = true,
                Secure = true
            });

            _accessTokenScopedStore = token;
        }

        public void SaveRefreshToken(string token, DateTimeOffset expires)
        {
            SaveToken("RefreshToken", token, 
            
            new CookieOptions
            {
                IsEssential = true,
                Expires = expires,
                HttpOnly = true,
                Secure = true
            }
            
            
            );

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