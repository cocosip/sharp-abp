using SharpAbp.Abp.AspNetCore.Http;
using System.Net;

namespace SharpAbp.Abp.AspNetCore.IpAccessControl
{
    public interface IIpAccessControlService
    {
        IpAccessDecision EvaluateCurrentRequest();

        IpAccessDecision Evaluate(HttpRequestInfo requestInfo);

        IpAccessDecision Evaluate(IPAddress? clientIpAddress);
    }
}
