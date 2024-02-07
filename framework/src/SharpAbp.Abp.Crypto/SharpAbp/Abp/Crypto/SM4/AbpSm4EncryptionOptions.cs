namespace SharpAbp.Abp.Crypto.SM4
{
    public class AbpSm4EncryptionOptions
    {
        public byte[] DefaultIv { get; set; }
        public string DefaultMode { get; set; }
        public string DefaultPadding { get; set; }
    }
}
