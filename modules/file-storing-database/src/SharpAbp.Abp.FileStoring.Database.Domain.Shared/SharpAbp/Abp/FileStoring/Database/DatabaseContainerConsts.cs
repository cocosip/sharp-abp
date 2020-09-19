namespace SharpAbp.Abp.FileStoring.Database
{
    public static class DatabaseContainerConsts
    {
        /// <summary>
        /// Default value: 128.
        /// </summary>
        public static int MaxNameLength { get; set; } = 128;

        /// <summary>
        /// Default value: 512
        /// </summary>
        public static int MaxHttpServerLength { get; set; } = 256;
    }
}
