﻿namespace SharpAbp.Abp.FileStoringManagement
{
    /// <summary>
    /// Data transfer object that represents a file storing provider.
    /// Used for listing and discovering available file storing providers in the system.
    /// </summary>
    public class ProviderDto
    {
        /// <summary>
        /// Gets or sets the name of the file storing provider.
        /// This represents the unique identifier for the provider type (e.g., "FileSystem", "Aliyun", "AWS", etc.).
        /// </summary>
        public string Provider { get; set; }
    }
}
