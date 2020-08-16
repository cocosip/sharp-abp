namespace SharpAbp.Abp.FileStoring
{
    public static class FileProviderNames
    {
        /// <summary>
        /// Local filesystem
        /// </summary>
        public static string FileSystem { get; set; } = "FileSystem";

        /// <summary>
        /// FastDFS provider name
        /// </summary>
        public static string FastDFS { get; set; } = "FastDFS";

        /// <summary>
        /// S3 provider name
        /// </summary>
        public static string S3 { get; set; } = "S3";

    }
}
