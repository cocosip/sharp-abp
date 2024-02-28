﻿using System;

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
        public string EncryptionAlgo { get; set; } = AbpTransformSecurityNames.RSA;

        /// <summary>
        /// Expires timespan
        /// </summary>
        public TimeSpan Expires { get; set; } = TimeSpan.FromSeconds(600);
    }
}
