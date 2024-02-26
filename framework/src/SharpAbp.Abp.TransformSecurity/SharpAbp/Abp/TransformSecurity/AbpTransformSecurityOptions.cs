namespace SharpAbp.Abp.TransformSecurity
{
    public class AbpTransformSecurityOptions
    {
        /// <summary>
        /// Enable security or not。 default: false
        /// </summary>
        public bool Enabled { get; set; } = false;

        /// <summary>
        /// Encryption algorithm. default: RSA  (RSA/SM2)
        /// </summary>
        public string EncryptionAlgo { get; set; } = "RSA";
    }
}
