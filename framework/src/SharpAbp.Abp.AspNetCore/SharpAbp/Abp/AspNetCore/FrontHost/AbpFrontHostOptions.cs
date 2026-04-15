using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace SharpAbp.Abp.AspNetCore.FrontHost
{
    public class AbpFrontHostOptions
    {
        public List<FrontApplication> Apps { get; }

        public AbpFrontHostOptions()
        {
            Apps = [];
        }

        public AbpFrontHostOptions Configure(IConfiguration configuration, string contentRootPath)
        {
            var contentRootFullPath = Path.GetFullPath(contentRootPath);
            var appConfigurations = configuration
                .GetSection("FrontHostOptions:Apps")
                .Get<List<FrontApplicationEntry>>();

            if (appConfigurations != null)
            {
                foreach (var appConfiguration in appConfigurations)
                {
                    var app = new FrontApplication()
                    {
                        Name = appConfiguration.Name,
                        RootPath = ResolveChildPath(contentRootFullPath, appConfiguration.RootPaths),
                    };
                    Apps.Add(app);

                    foreach (var p in appConfiguration.Pages)
                    {
                        var page = new FrontApplicationPage
                        {
                            Route = p.Route,
                            ContentType = p.ContentType,
                            Path = ResolveChildPath(app.RootPath!, p.Paths)
                        };
                        app.Pages.Add(page);
                    }

                    foreach (var s in appConfiguration.StaticDirs)
                    {
                        var staticDir = new FrontApplicationStaticDirectory()
                        {
                            RequestPath = s.RequestPath,
                            Path = ResolveChildPath(app.RootPath!, s.Paths)
                        };
                        app.StaticDirs.Add(staticDir);
                    }
                }
            }
            return this;
        }

        private static string ResolveChildPath(string root, params string[] paths)
        {
            var rootFullPath = Path.GetFullPath(root);
            if (paths == null || paths.Length == 0)
            {
                return rootFullPath;
            }

            var array = new string[paths.Length + 1];
            array[0] = rootFullPath;

            for (var i = 0; i < paths.Length; i++)
            {
                var normalizedPath = NormalizeConfiguredPath(paths[i]);
                if (IsAbsoluteConfiguredPath(normalizedPath))
                {
                    throw new InvalidOperationException($"Configured front-host path '{paths[i]}' must be relative to root '{rootFullPath}'.");
                }

                array[i + 1] = normalizedPath;
            }

            var fullPath = Path.GetFullPath(Path.Combine(array));

            if (!IsPathWithinRoot(rootFullPath, fullPath))
            {
                throw new InvalidOperationException($"Configured front-host path '{fullPath}' must stay within root '{rootFullPath}'.");
            }

            EnsurePathIsSafe(rootFullPath, fullPath);

            return fullPath;
        }

        internal static void EnsurePathIsSafe(string rootFullPath, string fullPath)
        {
            var normalizedRoot = Path.GetFullPath(rootFullPath);
            var normalizedPath = Path.GetFullPath(fullPath);

            if (!IsPathWithinRoot(normalizedRoot, normalizedPath))
            {
                throw new InvalidOperationException($"Configured front-host path '{normalizedPath}' must stay within root '{normalizedRoot}'.");
            }

            EnsurePathDoesNotUseReparsePoints(normalizedRoot, normalizedPath);
        }

        private static string NormalizeConfiguredPath(string path)
        {
            return (path ?? string.Empty)
                .Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar)
                .Replace('\\', Path.DirectorySeparatorChar);
        }

        private static bool IsAbsoluteConfiguredPath(string path)
        {
            if (Path.IsPathRooted(path))
            {
                return true;
            }

            if (path.StartsWith(@"\\", StringComparison.Ordinal) || path.StartsWith("//", StringComparison.Ordinal))
            {
                return true;
            }

            return path.Length >= 3
                && char.IsLetter(path[0])
                && path[1] == ':'
                && (path[2] == '\\' || path[2] == '/');
        }

        private static bool IsPathWithinRoot(string rootFullPath, string fullPath)
        {
            var normalizedRoot = rootFullPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                + Path.DirectorySeparatorChar;
            var comparison = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? StringComparison.OrdinalIgnoreCase
                : StringComparison.Ordinal;
            return fullPath.StartsWith(normalizedRoot, comparison)
                || string.Equals(fullPath, rootFullPath, comparison);
        }

        private static void EnsurePathDoesNotUseReparsePoints(string rootFullPath, string fullPath)
        {
            ValidatePathSegmentIfExists(rootFullPath);

            var relativePath = Path.GetRelativePath(rootFullPath, fullPath);
            if (string.IsNullOrWhiteSpace(relativePath) || relativePath == ".")
            {
                return;
            }

            var currentPath = rootFullPath;
            var segments = relativePath.Split([Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar], StringSplitOptions.RemoveEmptyEntries);
            foreach (var segment in segments)
            {
                currentPath = Path.Combine(currentPath, segment);
                if (!File.Exists(currentPath) && !Directory.Exists(currentPath))
                {
                    break;
                }

                ValidatePathSegmentIfExists(currentPath);
            }
        }

        private static void ValidatePathSegmentIfExists(string path)
        {
            if (!File.Exists(path) && !Directory.Exists(path))
            {
                return;
            }

            var attributes = File.GetAttributes(path);
            if ((attributes & FileAttributes.ReparsePoint) != 0)
            {
                throw new InvalidOperationException($"Configured front-host path '{path}' cannot use symbolic links or reparse points.");
            }
        }


    }
}
