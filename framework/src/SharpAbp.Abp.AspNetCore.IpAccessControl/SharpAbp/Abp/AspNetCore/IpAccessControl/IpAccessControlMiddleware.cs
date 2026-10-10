using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace SharpAbp.Abp.AspNetCore.IpAccessControl
{
    public class IpAccessControlMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IIpAccessControlService _ipAccessControlService;

        public IpAccessControlMiddleware(
            RequestDelegate next,
            IIpAccessControlService ipAccessControlService)
        {
            _next = next;
            _ipAccessControlService = ipAccessControlService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var decision = _ipAccessControlService.EvaluateCurrentRequest();
            if (!decision.IsAllowed)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return;
            }

            await _next(context);
        }
    }
}
