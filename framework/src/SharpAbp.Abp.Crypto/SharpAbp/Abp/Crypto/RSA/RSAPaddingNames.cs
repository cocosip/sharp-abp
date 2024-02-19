namespace SharpAbp.Abp.Crypto.RSA
{
    public static class RSAPaddingNames
    {
        public const string None = "RSA/NONE";
        public const string PKCS1Padding = "RSA/PKCS1Padding";
        public const string OAEPPadding = "RSA/OAEPPadding";
        public const string OAEPSHA1Padding = "RSA/OAEPPadding/SHA1";
        public const string OAEPSHA256Padding = "RSA/OAEPPadding/SHA256";
        public const string OAEPSHA384Padding = "RSA/OAEPPadding/SHA384";
        public const string OAEPSHA512Padding = "RSA/OAEPPadding/SHA512";
        public const string ISO9796d1Padding = "RSA/ISO9796d1";
    }
}
