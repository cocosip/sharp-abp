using System.Collections.Generic;

namespace SharpAbp.Abp.AspNetCore.FrontHost
{
    public class FrontApplicationEntry
    {
        /// <summary>
        /// 应用名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Web应用的根目录
        /// </summary>
        public string[] RootPaths { get; set; }

        /// <summary>
        /// 页面
        /// </summary>
        public List<FrontApplicationPageEntry> Pages { get; set; }

        /// <summary>
        /// 静态目录
        /// </summary>
        public List<FrontApplicationStaticDirectoryEntry> StaticDirs { get; set; }

        public FrontApplicationEntry()
        {
            RootPaths = [];
            Pages = [];
            StaticDirs = [];
        }
    }

    public class FrontApplicationPageEntry
    {
        public string Route { get; set; }
        public string ContentType { get; set; }
        public string[] Paths { get; set; }

        public FrontApplicationPageEntry()
        {
            Paths = [];
        }
    }

    /// <summary>
    /// 静态目录
    /// </summary>
    public class FrontApplicationStaticDirectoryEntry
    {
        public string RequestPath { get; set; }
        public string[] Paths { get; set; }

        public FrontApplicationStaticDirectoryEntry()
        {
            Paths = [];
        }
    }
}
