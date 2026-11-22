using System;
using System.Formats.Asn1;
using System.Security.Cryptography;
using SharpAbp.Abp.TransformSecurity;

namespace SharpAbp.Abp.TransformSecurityManagement
{
    internal static class RsaCredentialKeyNormalizer
    {
        public static void Normalize(SecurityCredential credential)
        {
            credential.PublicKey = Normalize(credential.PublicKey, false);
            credential.PrivateKey = Normalize(credential.PrivateKey, true);
        }

        private static string Normalize(string key, bool privateKey)
        {
            var isPem = PemEncoding.TryFind(key, out _);
            if (!isPem && !privateKey) return key;
            byte[] decoded = null;
            byte[] exported = null;
            try
            {
                if (!isPem)
                {
                    decoded = Convert.FromBase64String(key);
                    var sequence = new AsnReader(decoded, AsnEncodingRules.BER).ReadSequence();
                    sequence.ReadIntegerBytes();
                    // PKCS#1 places the modulus after the version; PKCS#8 places its algorithm identifier there.
                    if (sequence.PeekTag().HasSameClassAndValue(Asn1Tag.Integer)) return key;
                }

                using var rsa = RSA.Create();
                if (isPem)
                {
                    rsa.ImportFromPem(key);
                }
                else
                {
                    rsa.ImportPkcs8PrivateKey(decoded, out _);
                }

                exported = privateKey ? rsa.ExportRSAPrivateKey() : rsa.ExportSubjectPublicKeyInfo();
                return Convert.ToBase64String(exported);
            }
            finally
            {
                if (decoded != null) CryptographicOperations.ZeroMemory(decoded);
                if (exported != null) CryptographicOperations.ZeroMemory(exported);
            }
        }
    }
}
