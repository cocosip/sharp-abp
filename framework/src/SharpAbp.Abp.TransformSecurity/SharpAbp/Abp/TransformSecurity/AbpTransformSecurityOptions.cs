using System;
using System.Collections.Generic;

namespace SharpAbp.Abp.TransformSecurity
{
    /// <summary>
    /// Configuration options for ABP Transform Security functionality
    /// </summary>
    public class AbpTransformSecurityOptions
    {
        /// <summary>
        /// Gets or sets whether security is enabled. Default: false
        /// </summary>
        public bool Enabled { get; set; } = false;

        /// <summary>
        /// Gets or sets the encryption algorithm. Default: RSA (supported algorithms: RSA/SM2)
        /// </summary>
        public string EncryptionAlgo { get; set; } = AbpTransformSecurityNames.RSA;

        /// <summary>
        /// Gets or sets the expiration timespan for security credentials
        /// </summary>
        public TimeSpan Expires { get; set; } = TimeSpan.FromSeconds(600);

        /// <summary>
        /// Gets or sets the list of supported business types
        /// </summary>
        public List<string> BizTypes { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbpTransformSecurityOptions"/> class
        /// </summary>
        public AbpTransformSecurityOptions()
        {
            BizTypes = [];
        }
    }
}
