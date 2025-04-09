using Org.BouncyCastle.Crypto.Engines;
using SharpAbp.Abp.Crypto.RSA;
using SharpAbp.Abp.Crypto.SM2;
using System;
using Volo.Abp.Data;

namespace SharpAbp.Abp.TransformSecurity
{
    public static class SecurityCredentialExtensions
    {
        public static bool IsRSA(this SecurityCredential key)
        {
            return key.KeyType == "RSA";
        }

        public static bool IsSM2(this SecurityCredential key)
        {
            return key.KeyType == "SM2";
        }

        public static string? GetReferenceId(this SecurityCredential key)
        {
            return key.GetProperty("ReferenceId", "");
        }

        public static SecurityCredential SetReferenceId(this SecurityCredential key, string id)
        {
            return key.SetProperty("ReferenceId", id);
        }

        public static int GetRSAKeySize(this SecurityCredential key)
        {
            return key.GetProperty("RSA_KeySize", 2048);
        }

        public static SecurityCredential SetRSAKeySize(this SecurityCredential key, int keySize)
        {
            return key.SetProperty("RSA_KeySize", keySize, false);
        }

        public static string? GetRSAPadding(this SecurityCredential key)
        {
            return key.GetProperty("RSA_Padding", RSAPaddingNames.None);
        }

        public static SecurityCredential SetRSAPadding(this SecurityCredential key, string padding)
        {
            return key.SetProperty("RSA_Padding", padding);
        }

        public static string? GetSM2Curve(this SecurityCredential key)
        {
            return key.GetProperty("SM2_Curve", Sm2EncryptionNames.CurveSm2p256v1);
        }

        public static SecurityCredential SetSM2Curve(this SecurityCredential key, string curve)
        {
            return key.SetProperty("SM2_Curve", curve);
        }

        public static SM2Engine.Mode GetSM2Mode(this SecurityCredential key)
        {

            return (SM2Engine.Mode)Enum.Parse(typeof(SM2Engine.Mode), key.GetProperty("SM2_Mode", SM2Engine.Mode.C1C2C3.ToString()));
        }

        public static SecurityCredential SetSM2Mode(this SecurityCredential key, SM2Engine.Mode mode)
        {
            return key.SetProperty("SM2_Mode", mode.ToString());
        }
    }
}
