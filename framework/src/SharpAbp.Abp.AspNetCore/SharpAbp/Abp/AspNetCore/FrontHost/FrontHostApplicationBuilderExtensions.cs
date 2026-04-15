using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading;
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
                //Page File Registration
                foreach (var page in app.Pages)
                {
                    builder.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGet(page.Route!, ctx => MapPage(app.RootPath!, page, ctx));
                    });
                }

                //Static File Directory Registration
                foreach (var staticDir in app.StaticDirs)
                {
                    if (string.IsNullOrWhiteSpace(staticDir.Path))
                    {
                        throw new InvalidOperationException($"Front-host static directory path is missing for app '{app.Name}'.");
                    }

                    if (!Directory.Exists(staticDir.Path))
                    {
                        throw new DirectoryNotFoundException($"Front-host static directory '{staticDir.Path}' for app '{app.Name}' was not found.");
                    }

                    AbpFrontHostOptions.EnsurePathIsSafe(app.RootPath!, staticDir.Path);

                    builder.UseStaticFiles(new StaticFileOptions()
                    {
                        FileProvider = new PhysicalFileProvider(staticDir.Path!),
                        RequestPath = staticDir.RequestPath,
                        ContentTypeProvider = new FileExtensionContentTypeProvider()
                    });
                }
            }

            return builder;
        }

        private static async Task MapPage(string rootPath, FrontApplicationPage page, HttpContext context)
        {
            if (string.IsNullOrWhiteSpace(page.Path) || !File.Exists(page.Path))
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }

            try
            {
                AbpFrontHostOptions.EnsurePathIsSafe(rootPath, page.Path);
            }
            catch (InvalidOperationException)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }

            if (!string.IsNullOrWhiteSpace(page.ContentType))
            {
                context.Response.ContentType = page.ContentType;
            }

            await context.Response.SendFileAsync(page.Path, 0, null, context.RequestAborted);
        }
    }
}
