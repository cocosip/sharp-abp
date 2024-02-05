using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using System.IO;
using System.Threading.Tasks;

namespace SharpAbp.Abp.AspNetCore.FrontHost
{
    public static class FrontHostApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseFrontHost(this IApplicationBuilder builder)
        {
            var options = builder.ApplicationServices.GetRequiredService<IOptions<AbpFrontHostOptions>>().Value;

            var webHostEnvironment = builder.ApplicationServices.GetRequiredService<IWebHostEnvironment>();

            foreach (var app in options.Apps)
            {
                //页面文件注册
                foreach (var page in app.Pages)
                {
                    builder.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGet(page.Route, ctx => MapPage(page, ctx));
                    });
                }

                //静态文件目录注册
                foreach (var staticDir in app.StaticDirs)
                {
                    builder.UseStaticFiles(new StaticFileOptions()
                    {
                        FileProvider = new PhysicalFileProvider(staticDir.Path),
                        RequestPath = staticDir.RequestPath,
                        ContentTypeProvider = new FileExtensionContentTypeProvider()
                    });
                }
            }

            return builder;
        }

        private static Task MapPage(FrontApplicationPage page, HttpContext context)
        {
            var buffer = File.ReadAllBytes(page.Path);
            context.Response.ContentType = page.ContentType;
            return context.Response.Body.WriteAsync(buffer, 0, buffer.Length);
        }
    }
}
