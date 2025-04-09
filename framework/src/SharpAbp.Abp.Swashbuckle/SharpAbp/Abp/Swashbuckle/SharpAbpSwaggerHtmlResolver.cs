using Swashbuckle.AspNetCore.SwaggerUI;
using System.IO;
using System.Reflection;
using System.Text;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Swashbuckle;

namespace SharpAbp.Abp.Swashbuckle
{
    [Dependency(ReplaceServices = true)]
    [ExposeServices(typeof(ISwaggerHtmlResolver))]
    public class SharpAbpSwaggerHtmlResolver : ISwaggerHtmlResolver, ITransientDependency
    {
        public virtual Stream Resolver()
        {
            var stream = typeof(SwaggerUIOptions).GetTypeInfo().Assembly
                .GetManifestResourceStream("Swashbuckle.AspNetCore.SwaggerUI.index.html");

            var html = new StreamReader(stream!)
                .ReadToEnd()
                .Replace("SwaggerUIBundle(configObject)", "abp.SwaggerUIBundle(configObject)");

            return new MemoryStream(Encoding.UTF8.GetBytes(html));
        }
    }
}
