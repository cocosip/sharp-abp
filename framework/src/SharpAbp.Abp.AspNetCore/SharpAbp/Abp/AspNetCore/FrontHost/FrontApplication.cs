using System.Collections.Generic;

namespace SharpAbp.Abp.AspNetCore.FrontHost
{
    public class FrontApplication
    {
        /// <summary>
        /// 应用名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Web应用的根目录
        /// </summary>
        public string RootPath { get; set; }

        /// <summary>
        /// 页面
        /// </summary>
        public List<FrontApplicationPage> Pages { get; set; }

        /// <summary>
        /// 静态目录
        /// </summary>
        public List<FrontApplicationStaticDirectory> StaticDirs { get; set; }

        public FrontApplication()
        {
            Pages = [];
            StaticDirs = [];
        }
    }


    /// <summary>
    /// 前端应用页面
    /// </summary>
    public class FrontApplicationPage
    {
        public string Route { get; set; }
        public string ContentType { get; set; }
        public string Path { get; set; }
    }

    /// <summary>
    /// 静态目录
    /// </summary>
    public class FrontApplicationStaticDirectory
    {
        public string RequestPath { get; set; }
        public string Path { get; set; }
    }
}
