namespace SharpAbp.Abp.CryptoVault
{
    public static class SM2CredsConsts
    {
        /// <summary>
        /// Default value: 64
        /// </summary>
        public static int MaxIdentifierLength { get; set; } = 64;

        /// <summary>
        /// Default value: 32
        /// </summary>
        public static int MaxCurveLength { get; set; } = 32;

        /// <summary>
        /// Default value: 256
        /// </summary>
        public static int MaxDescriptionLength { get; set; } = 256;

        /// <summary>
        /// Default value: 512
        /// </summary>
        public static int MaxPassPhraseLength { get; set; } = 512;

        /// <summary>
        /// Default value: 512
        /// </summary>
        public static int MaxSaltLength { get; set; } = 512;
    }
}
