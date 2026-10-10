using SharpAbp.Abp.AspNetCore.Http;
using System.Net;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.AspNetCore.IpAccessControl
{
    public class ClientIpAddressAccessor : IClientIpAddressAccessor, ITransientDependency
    {
        protected IHttpRequestInfoAccessor RequestInfoAccessor { get; }

        public ClientIpAddressAccessor(IHttpRequestInfoAccessor requestInfoAccessor)
        {
            RequestInfoAccessor = requestInfoAccessor;
        }

        public virtual IPAddress? GetClientIpAddress()
        {
            var requestInfo = RequestInfoAccessor.GetRequestInfo();
            if (requestInfo.ResolutionStatus != RequestInfoResolutionStatus.Resolved)
            {
                return null;
            }

            return Normalize(requestInfo.ClientIpAddress);
        }

        protected virtual IPAddress? Normalize(IPAddress? address)
        {
            return address?.IsIPv4MappedToIPv6 == true ? address.MapToIPv4() : address;
        }
    }
}
