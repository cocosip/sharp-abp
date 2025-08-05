using System.Collections.Generic;

namespace SharpAbp.Abp.AspNetCore.FrontHost
{
    /// <summary>
    /// Represents an entry for a front-end application configuration.
    /// </summary>
    public class FrontApplicationEntry
    {
        /// <summary>
        /// Gets or sets the name of the application.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the root paths of the web application.
        /// </summary>
        public string[] RootPaths { get; set; }

        /// <summary>
        /// Gets or sets the pages of the application.
        /// </summary>
        public List<FrontApplicationPageEntry> Pages { get; set; }

        /// <summary>
        /// Gets or sets the static directories of the application.
        /// </summary>
        public List<FrontApplicationStaticDirectoryEntry> StaticDirs { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FrontApplicationEntry"/> class.
        /// </summary>
        public FrontApplicationEntry()
        {
            RootPaths = [];
            Pages = [];
            StaticDirs = [];
        }
    }

    /// <summary>
    /// Represents an entry for a front-end application page configuration.
    /// </summary>
    public class FrontApplicationPageEntry
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
        /// Gets or sets the paths of the page.
        /// </summary>
        public string[] Paths { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FrontApplicationPageEntry"/> class.
        /// </summary>
        public FrontApplicationPageEntry()
        {
            Paths = [];
        }
    }

    /// <summary>
    /// Represents an entry for a front-end application static directory configuration.
    /// </summary>
    public class FrontApplicationStaticDirectoryEntry
    {
        /// <summary>
        /// Gets or sets the request path of the static directory.
        /// </summary>
        public string? RequestPath { get; set; }

        /// <summary>
        /// Gets or sets the paths of the static directory.
        /// </summary>
        public string[] Paths { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FrontApplicationStaticDirectoryEntry"/> class.
        /// </summary>
        public FrontApplicationStaticDirectoryEntry()
        {
            Paths = [];
        }
    }
}
