using System.Collections.Generic;

namespace SharpAbp.Abp.AspNetCore.FrontHost
{
    /// <summary>
    /// Represents a front-end application configuration.
    /// </summary>
    public class FrontApplication
    {
        /// <summary>
        /// Gets or sets the name of the application.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the root path of the web application.
        /// </summary>
        public string? RootPath { get; set; }

        /// <summary>
        /// Gets or sets the pages of the application.
        /// </summary>
        public List<FrontApplicationPage> Pages { get; set; }

        /// <summary>
        /// Gets or sets the static directories of the application.
        /// </summary>
        public List<FrontApplicationStaticDirectory> StaticDirs { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FrontApplication"/> class.
        /// </summary>
        public FrontApplication()
        {
            Pages = [];
            StaticDirs = [];
        }
    }

    /// <summary>
    /// Represents a front-end application page configuration.
    /// </summary>
    public class FrontApplicationPage
    {
        /// <summary>
        /// Gets or sets the route of the page.
        /// </summary>
        public string? Route { get; set; }

        /// <summary>
        /// Gets or sets the content type of the page.
        /// </summary>
        public string? ContentType { get; set; }

        /// <summary>
        /// Gets or sets the path of the page.
        /// </summary>
        public string? Path { get; set; }
    }

    /// <summary>
    /// Represents a front-end application static directory configuration.
    /// </summary>
    public class FrontApplicationStaticDirectory
    {
        /// <summary>
        /// Gets or sets the request path of the static directory.
        /// </summary>
        public string? RequestPath { get; set; }

        /// <summary>
        /// Gets or sets the path of the static directory.
        /// </summary>
        public string? Path { get; set; }
    }
}
