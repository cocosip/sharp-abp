using System;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.CryptoVault
{
    public class RSACredsDto : AuditedEntityDto<Guid>
    {
        /// <summary>
        /// 唯一标志
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// RSA密钥长度 (1024,2048)
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// RSA 公钥(加密后)
        /// </summary>
        public string PublicKey { get; set; }

        /// <summary>
        /// RSA 私钥(加密后)
        /// </summary>
        public string PrivateKey { get; set; }

        /// <summary>
        /// 描述信息
        /// </summary>
        public string Description { get; set; }
    }
}
