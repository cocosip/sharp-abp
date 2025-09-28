﻿namespace SharpAbp.Abp.CryptoVault
{
    /// <summary>
    /// Configuration options for the AbpCryptoVault module.
    /// </summary>
    public class AbpCryptoVaultOptions
    {
        /// <summary>
        /// Gets or sets the number of RSA key pairs to generate.
        /// </summary>
        public int RSACount { get; set; }

        /// <summary>
        /// Gets or sets the RSA key size in bits (e.g., 1024, 2048, 4096).
        /// </summary>
        public int RSAKeySize { get; set; }

        /// <summary>
        /// Gets or sets the number of SM2 key pairs to generate.
        /// </summary>
        public int SM2Count { get; set; }

        /// <summary>
        /// Gets or sets the SM2 elliptic curve name (e.g., wapip192v1, sm2p256v1).
        /// </summary>
        public string SM2Curve { get; set; }
    }
}
