namespace SharpAbp.Abp.FileStoring
{
    public static class S3BucketConsts
    {
        /// <summary>
        /// Default value:64
        /// </summary>
        public static int MaxBucketNameLength { get; set; } = 64;

        /// <summary>
        /// Default value:256
        /// </summary>
        public static int MaxServerUrlLength { get; set; } = 256;

        /// <summary>
        /// Default value:256
        /// </summary>
        public static int MaxAccessKeyIdLength { get; set; } = 256;

        /// <summary>
        /// Default value:256
        /// </summary>
        public static int MaxSecretAccessKeyLength { get; set; } = 256;

        /// <summary>
        /// Default value:32
        /// </summary>
        public static int MaxVendorVersionLength { get; set; } = 32;

        /// <summary>
        /// Default value:10
        /// </summary>
        public static int MaxSignatureVersionLength { get; set; } = 10;

        /// <summary>
        /// Default value:256
        /// </summary>
        public static int MaxAccessUrlLength { get; set; } = 256;
    }
}
