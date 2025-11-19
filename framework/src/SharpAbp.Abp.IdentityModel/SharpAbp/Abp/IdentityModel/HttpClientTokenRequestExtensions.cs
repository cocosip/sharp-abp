using Duende.IdentityModel;
using Duende.IdentityModel.Client;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.Abp.IdentityModel
{
    public static class HttpClientTokenRequestExtensions
    {
        /// <summary>
        /// ExternalCredentials token
        /// </summary>
        /// <param name="client"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<TokenResponse> RequestExternalCredentialsTokenAsync(
            this HttpMessageInvoker client,
            ExternalCredentialsTokenRequest request,
            CancellationToken cancellationToken = default)
        {

            var clone = request.Clone();

            clone.Parameters.AddRequired(OidcConstants.TokenRequest.GrantType, ExternalCredentialsConstants.GrantType);
            clone.Parameters.AddRequired(ExternalCredentialsConstants.LoginProvider, request.LoginProvider);
            clone.Parameters.AddRequired(ExternalCredentialsConstants.ProviderKey, request.ProviderKey);
            
            clone.Parameters.AddOptional(OidcConstants.TokenRequest.Scope, request.Scope);

            foreach (var resource in request.Resource)
            {
                clone.Parameters.AddRequired(OidcConstants.TokenRequest.Resource, resource, allowDuplicates: true);
            }
            return await client.RequestTokenAsync(clone, cancellationToken).ConfigureAwait(false);
        }

        internal static async Task<TokenResponse> RequestTokenAsync(
            this HttpMessageInvoker client,
            ProtocolRequest request,
            CancellationToken cancellationToken = default)
        {
            request.Prepare();
            request.Method = HttpMethod.Post;

            HttpResponseMessage response;
            try
            {
                response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return ProtocolResponse.FromException<TokenResponse>(ex);
            }

            return await ProtocolResponse.FromHttpResponseAsync<TokenResponse>(response).ConfigureAwait(false);
        }
    }
}
