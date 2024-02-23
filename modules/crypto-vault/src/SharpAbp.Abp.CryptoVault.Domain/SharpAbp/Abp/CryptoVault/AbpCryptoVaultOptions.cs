namespace SharpAbp.Abp.CryptoVault
{
    public class AbpCryptoVaultOptions
    {
        public int RSACount { get; set; }
        public int RSAKeySize { get; set; }
        public int SM2Count { get; set; }
        public string SM2Curve { get; set; }
    }
}
