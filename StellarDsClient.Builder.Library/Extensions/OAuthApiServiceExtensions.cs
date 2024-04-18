﻿using Microsoft.IdentityModel.JsonWebTokens;
using StellarDsClient.Sdk;

namespace StellarDsClient.Builder.Library.Extensions
{
    internal static class OAuthApiServiceExtensions
    {
        internal static async Task<string> GetAccessToken(this OAuthApiService oAuthApiService, string code)
        {
            var tokens = await oAuthApiService.GetTokensAsync(code);

            new JsonWebTokenHandler().ReadJsonWebToken(tokens.AccessToken);
            
            return tokens.AccessToken;
        }
    }
}