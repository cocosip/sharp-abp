using System.Net;

namespace SharpAbp.Abp.AspNetCore.IpAccessControl
{
    public interface IClientIpAddressAccessor
    {
        IPAddress? GetClientIpAddress();
    }
}
