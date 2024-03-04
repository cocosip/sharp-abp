namespace SharpAbp.Abp.TransformSecurityManagement
{
    public static class SecurityCredentialInfoConsts
    {
        /// <summary>
        /// Default value: 64
        /// </summary>
        public static int MaxIdentifierLength { get; set; } = 64;

        /// <summary>
        /// Default value: 32
        /// </summary>
        public static int MaxKeyTypeLength { get; set; } = 32;

        /// <summary>
        /// Default value: 32
        /// </summary>
        public static int MaxBizTypeLength { get; set; } = 32;

        /// <summary>
        /// Default value: 256
        /// </summary>
        public static int MaxDescriptionLength { get; set; } = 256;
    }
}
