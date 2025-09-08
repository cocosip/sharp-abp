using Org.BouncyCastle.Crypto.Engines;
using SharpAbp.Abp.Crypto.RSA;
using SharpAbp.Abp.Crypto.SM2;
using System;
using Volo.Abp.Data;

namespace SharpAbp.Abp.TransformSecurity
{
    /// <summary>
    /// Extension methods for SecurityCredential to provide convenient access to security credential properties.
    /// </summary>
    public static class SecurityCredentialExtensions
    {
        /// <summary>
        /// Property key constants for security credential properties.
        /// </summary>
        private static class PropertyKeys
        {
            /// <summary>
            /// Reference identifier property key.
            /// </summary>
            public const string ReferenceId = "ReferenceId";
            
            /// <summary>
            /// RSA key size property key.
            /// </summary>
            public const string RsaKeySize = "RSA_KeySize";
            
            /// <summary>
            /// RSA padding property key.
            /// </summary>
            public const string RsaPadding = "RSA_Padding";
            
            /// <summary>
            /// SM2 curve property key.
            /// </summary>
            public const string Sm2Curve = "SM2_Curve";
            
            /// <summary>
            /// SM2 mode property key.
            /// </summary>
            public const string Sm2Mode = "SM2_Mode";
            

        }
        /// <summary>
        /// Determines whether the security credential uses RSA encryption algorithm.
        /// </summary>
        /// <param name="key">The security credential.</param>
        /// <returns>True if the credential uses RSA algorithm; otherwise, false.</returns>
        public static bool IsRSA(this SecurityCredential key)
        {
            return key.KeyType == AbpTransformSecurityNames.RSA;
        }

        /// <summary>
        /// Determines whether the security credential uses SM2 encryption algorithm.
        /// </summary>
        /// <param name="key">The security credential.</param>
        /// <returns>True if the credential uses SM2 algorithm; otherwise, false.</returns>
        public static bool IsSM2(this SecurityCredential key)
        {
            return key.KeyType == AbpTransformSecurityNames.SM2;
        }

        /// <summary>
        /// Gets the reference identifier from the security credential.
        /// </summary>
        /// <param name="key">The security credential.</param>
        /// <returns>The reference identifier or empty string if not found.</returns>
        public static string? GetReferenceId(this SecurityCredential key)
        {
            return key.GetProperty(PropertyKeys.ReferenceId, "");
        }

        /// <summary>
        /// Sets the reference identifier for the security credential.
        /// </summary>
        /// <param name="key">The security credential.</param>
        /// <param name="id">The reference identifier to set.</param>
        /// <returns>The updated security credential.</returns>
        public static SecurityCredential SetReferenceId(this SecurityCredential key, string id)
        {
            return key.SetProperty(PropertyKeys.ReferenceId, id);
        }

        /// <summary>
        /// Gets the RSA key size from the security credential.
        /// </summary>
        /// <param name="key">The security credential.</param>
        /// <returns>The RSA key size, defaults to 2048 if not specified.</returns>
        public static int GetRSAKeySize(this SecurityCredential key)
        {
            return key.GetProperty(PropertyKeys.RsaKeySize, 2048);
        }

        /// <summary>
        /// Sets the RSA key size for the security credential.
        /// </summary>
        /// <param name="key">The security credential.</param>
        /// <param name="keySize">The RSA key size to set.</param>
        /// <returns>The updated security credential.</returns>
        public static SecurityCredential SetRSAKeySize(this SecurityCredential key, int keySize)
        {
            return key.SetProperty(PropertyKeys.RsaKeySize, keySize, false);
        }

        /// <summary>
        /// Gets the RSA padding scheme from the security credential.
        /// </summary>
        /// <param name="key">The security credential.</param>
        /// <returns>The RSA padding scheme, defaults to None if not specified.</returns>
        public static string? GetRSAPadding(this SecurityCredential key)
        {
            return key.GetProperty(PropertyKeys.RsaPadding, RSAPaddingNames.None);
        }

        /// <summary>
        /// Sets the RSA padding scheme for the security credential.
        /// </summary>
        /// <param name="key">The security credential.</param>
        /// <param name="padding">The RSA padding scheme to set.</param>
        /// <returns>The updated security credential.</returns>
        public static SecurityCredential SetRSAPadding(this SecurityCredential key, string padding)
        {
            return key.SetProperty(PropertyKeys.RsaPadding, padding);
        }

        /// <summary>
        /// Gets the SM2 curve from the security credential.
        /// </summary>
        /// <param name="key">The security credential.</param>
        /// <returns>The SM2 curve, defaults to CurveSm2p256v1 if not specified.</returns>
        public static string? GetSM2Curve(this SecurityCredential key)
        {
            return key.GetProperty(PropertyKeys.Sm2Curve, Sm2EncryptionNames.CurveSm2p256v1);
        }

        /// <summary>
        /// Sets the SM2 curve for the security credential.
        /// </summary>
        /// <param name="key">The security credential.</param>
        /// <param name="curve">The SM2 curve to set.</param>
        /// <returns>The updated security credential.</returns>
        public static SecurityCredential SetSM2Curve(this SecurityCredential key, string curve)
        {
            return key.SetProperty(PropertyKeys.Sm2Curve, curve);
        }

        /// <summary>
        /// Gets the SM2 engine mode from the security credential.
        /// </summary>
        /// <param name="key">The security credential.</param>
        /// <returns>The SM2 engine mode, defaults to C1C2C3 if not specified.</returns>
        public static SM2Engine.Mode GetSM2Mode(this SecurityCredential key)
        {
            return (SM2Engine.Mode)Enum.Parse(typeof(SM2Engine.Mode), key.GetProperty(PropertyKeys.Sm2Mode, SM2Engine.Mode.C1C2C3.ToString()));
        }

        /// <summary>
        /// Sets the SM2 engine mode for the security credential.
        /// </summary>
        /// <param name="key">The security credential.</param>
        /// <param name="mode">The SM2 engine mode to set.</param>
        /// <returns>The updated security credential.</returns>
        public static SecurityCredential SetSM2Mode(this SecurityCredential key, SM2Engine.Mode mode)
        {
            return key.SetProperty(PropertyKeys.Sm2Mode, mode.ToString());
        }
    }
}
