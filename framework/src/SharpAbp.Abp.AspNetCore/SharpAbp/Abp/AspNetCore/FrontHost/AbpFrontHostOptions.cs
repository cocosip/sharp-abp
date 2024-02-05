using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;

namespace SharpAbp.Abp.AspNetCore.FrontHost
{
    public class AbpFrontHostOptions
    {
        public List<FrontApplication> Apps { get; }

        public AbpFrontHostOptions()
        {
            Apps = new List<FrontApplication>();
        }

        public AbpFrontHostOptions Configure(IConfiguration configuration, string contentRootPath)
        {
            var appConfigurations = configuration
                .GetSection("FrontHostOptions:Apps")
                .Get<List<FrontApplicationEntry>>();

            foreach (var appConfiguration in appConfigurations)
            {
                var app = new FrontApplication()
                {
                    Name = appConfiguration.Name,
                    RootPath = PathCombine(contentRootPath, appConfiguration.RootPath),
                };
                Apps.Add(app);

                foreach (var p in appConfiguration.Pages)
                {
                    var page = new FrontApplicationPage
                    {
                        Route = p.Route,
                        ContentType = p.ContentType,
                        Path = PathCombine(app.RootPath, p.Path)
                    };
                    app.Pages.Add(page);
                }

                foreach (var s in appConfiguration.StaticDirs)
                {
                    var staticDir = new FrontApplicationStaticDirectory()
                    {
                        RequestPath = s.RequestPath,
                        Path = PathCombine(app.RootPath, s.Path)
                    };
                    app.StaticDirs.Add(staticDir);
                }
            }

            return this;
        }

        private static string PathCombine(string root, params string[] paths)
        {
            if (paths == null || paths.Length == 0)
            {
                return root;
            }
            var array = new string[paths.Length + 1];
            array[0] = root;
            Array.Copy(paths, 0, array, 1, paths.Length);
            return Path.Join(array);
        }


    }
}
