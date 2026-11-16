namespace SharpAbp.Abp.CryptoVault
{
    public static class RSACredsConsts
    {
        /// <summary>
        /// Default value: 64
        /// </summary>
        public static int MaxIdentifierLength { get; set; } = 64;

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

        /// <summary>
        /// Default value: 8192
        /// Key material is long text: the length must stay above 2000 so Oracle maps the column to NCLOB
        /// instead of NVARCHAR2(2000), which cannot hold an encrypted 3072/4096 bit key.
        /// </summary>
        public static int MaxPublicKeyLength { get; set; } = 8192;

        /// <summary>
        /// Default value: 8192
        /// Key material is long text: the length must stay above 2000 so Oracle maps the column to NCLOB
        /// instead of NVARCHAR2(2000), which cannot hold an encrypted 3072/4096 bit key.
        /// </summary>
        public static int MaxPrivateKeyLength { get; set; } = 8192;

    }
}
