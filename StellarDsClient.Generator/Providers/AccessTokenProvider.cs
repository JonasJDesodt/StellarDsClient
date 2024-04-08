using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StellarDsClient.Sdk.Abstractions;

namespace StellarDsClient.Generator.Providers
{
    internal class AccessTokenProvider : ITokenProvider
    {
        private string? _accessToken;

        public Task<string> Get()
        {
            return Task.FromResult(_accessToken ?? throw new NullReferenceException()); //todo: handle this better
        }

        public void Set(string accessToken)
        {
            _accessToken = accessToken;
        }
    }
}