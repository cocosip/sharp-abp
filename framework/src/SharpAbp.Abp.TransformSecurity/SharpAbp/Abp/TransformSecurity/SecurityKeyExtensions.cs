using Org.BouncyCastle.Crypto.Engines;
using SharpAbp.Abp.Crypto.RSA;
using SharpAbp.Abp.Crypto.SM2;
using Volo.Abp.Data;

namespace SharpAbp.Abp.TransformSecurity
{
    public static class SecurityKeyExtensions
    {
        public static bool IsRSA(this SecurityKey key)
        {
            return key.KeyType == "RSA";
        }

        public static bool IsSM2(this SecurityKey key)
        {
            return key.KeyType == "SM2";
        }

        public static int GetRSAKeySize(this SecurityKey key)
        {
            return key.GetProperty("RSA_KeySize", 2048);
        }

        public static SecurityKey SetRSAKeySize(this SecurityKey key, int keySize)
        {
            return key.SetProperty("RSA_KeySize", keySize, false);
        }

        public static string GetRSAPadding(this SecurityKey key)
        {
            return key.GetProperty("RSA_Padding", RSAPaddingNames.None);
        }

        public static SecurityKey SetRSAPadding(this SecurityKey key, string padding)
        {
            return key.SetProperty("RSA_Padding", padding);
        }

        public static string GetSM2Curve(this SecurityKey key)
        {
            return key.GetProperty("SM2_Curve", Sm2EncryptionNames.CurveSm2p256v1);
        }

        public static SecurityKey SetSM2Curve(this SecurityKey key, string curve)
        {
            return key.SetProperty("SM2_Curve", curve);
        }

        public static SM2Engine.Mode GetSM2Mode(this SecurityKey key)
        {
            return key.GetProperty("SM2_Mode", SM2Engine.Mode.C1C2C3);
        }

        public static SecurityKey SetSM2Mode(this SecurityKey key, SM2Engine.Mode mode)
        {
            return key.SetProperty("SM2_Mode", mode);
        }
    }
}
