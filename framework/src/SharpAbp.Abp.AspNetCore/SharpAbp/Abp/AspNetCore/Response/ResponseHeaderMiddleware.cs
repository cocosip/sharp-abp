using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.AspNetCore.Response
{
    public class ResponseHeaderMiddleware : IMiddleware, ITransientDependency
    {
        protected AbpHttpResponseHeaderOptions Options { get; }
        public ResponseHeaderMiddleware(IOptions<AbpHttpResponseHeaderOptions> options)
        {
            Options = options.Value;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (Options.Headers.Count > 0)
            {
                foreach (var header in Options.Headers)
                {
                    context.Response.Headers.Append(header.Key, header.Value);
                }
            }
            await next(context);
        }
    }
}
