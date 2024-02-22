namespace SharpAbp.Abp.CryptoVault
{
    public class ImportRSACredsDto
    {
        public int Size { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
    }
}
