using IdentityModel;
using IdentityModel.Client;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.IdentityModel;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.IdentityModel
{
    [Dependency(ReplaceServices = true)]
    public class AbpIdentityModelAuthenticationService : IdentityModelAuthenticationService
    {
        public AbpIdentityModelAuthenticationService(
            IOptions<AbpIdentityClientOptions> options,
            ICancellationTokenProvider cancellationTokenProvider,
            IHttpClientFactory httpClientFactory,
            ICurrentTenant currentTenant,
            IOptions<IdentityModelHttpRequestMessageOptions> identityModelHttpRequestMessageOptions,
            IDistributedCache<IdentityModelTokenCacheItem> tokenCache,
            IDistributedCache<IdentityModelDiscoveryDocumentCacheItem> discoveryDocumentCache) : base(
                options,
                cancellationTokenProvider,
                httpClientFactory,
                currentTenant,
                identityModelHttpRequestMessageOptions,
                tokenCache,
                discoveryDocumentCache)
        {

        }


        protected override async Task<TokenResponse> GetTokenResponse(IdentityClientConfiguration configuration)
        {
            using (var httpClient = HttpClientFactory.CreateClient(HttpClientName))
            {
                AddHeaders(httpClient);

                switch (configuration.GrantType)
                {
                    case OidcConstants.GrantTypes.ClientCredentials:
                        return await httpClient.RequestClientCredentialsTokenAsync(
                            await CreateClientCredentialsTokenRequestAsync(configuration),
                            CancellationTokenProvider.Token
                        );
                    case OidcConstants.GrantTypes.Password:
                        return await httpClient.RequestPasswordTokenAsync(
                            await CreatePasswordTokenRequestAsync(configuration),
                            CancellationTokenProvider.Token
                        );
                    case ExternalCredentialsConstants.GrantType:
                        return await httpClient.RequestExternalCredentialsTokenAsync(
                            await CreateExternalCredentialsTokenRequestAsync(configuration),
                            CancellationTokenProvider.Token
                        );
                    default:
                        throw new AbpException("Grant type was not implemented: " + configuration.GrantType);
                }
            }
        }

        protected virtual async Task<ExternalCredentialsTokenRequest> CreateExternalCredentialsTokenRequestAsync(IdentityClientConfiguration configuration)
        {
            var discoveryResponse = await GetDiscoveryResponse(configuration);
            var request = new ExternalCredentialsTokenRequest
            {
                Address = discoveryResponse.TokenEndpoint,
                ClientId = configuration.ClientId,
                ClientSecret = configuration.ClientSecret,
                Scope = configuration.Scope,
                LoginProvider = configuration.UserName,
                ProviderKey = configuration.UserPassword
            };

            IdentityModelHttpRequestMessageOptions.ConfigureHttpRequestMessage?.Invoke(request);

            await AddParametersToRequestAsync(configuration, request);

            return request;
        }

    }
}
