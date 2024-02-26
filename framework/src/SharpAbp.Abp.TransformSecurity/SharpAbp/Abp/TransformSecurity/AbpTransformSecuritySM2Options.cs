using Org.BouncyCastle.Crypto.Engines;
using SharpAbp.Abp.Crypto.SM2;


namespace SharpAbp.Abp.TransformSecurity
{
    public class AbpTransformSecuritySM2Options
    {
        /// <summary>
        /// Curve, default: sm2p256v1
        /// </summary>
        public string Curve { get; set; } = Sm2EncryptionNames.CurveSm2p256v1;

        /// <summary>
        /// Mode default: C1C2C3
        /// </summary>
        public SM2Engine.Mode Mode { get; set; } = SM2Engine.Mode.C1C2C3;
    }
}
