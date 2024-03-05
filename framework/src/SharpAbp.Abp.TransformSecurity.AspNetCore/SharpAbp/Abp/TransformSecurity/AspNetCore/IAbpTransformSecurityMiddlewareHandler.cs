using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.Abp.TransformSecurity.AspNetCore
{
    public interface IAbpTransformSecurityMiddlewareHandler
    {
        Task HandleAsync(HttpContext context, string identifier, CancellationToken cancellationToken = default);
    }
}
