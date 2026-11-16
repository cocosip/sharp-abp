namespace SharpAbp.Abp.FileStoring.Database
{
    public static class DatabaseFileConsts
    {
        /// <summary>
        /// Default value: 256.
        /// </summary>
        public static int MaxNameLength { get; set; } = 256;

        /// <summary>
        /// Default value: <see cref="int.MaxValue"/> (no module-level limit).
        /// The EF column is mapped with this value as max length so the content stays a LOB (BLOB on Oracle).
        /// </summary>
        public static int MaxContentLength { get; set; } = int.MaxValue;
    }
}
