using System;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.CryptoVault
{
    public class SM2CredsDto : AuditedEntityDto<Guid>
    {
        /// <summary>
        /// 唯一标志
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// 曲率名称,默认:sm2p256v1  (wapip192v1,sm2p256v1)
        /// </summary>
        public string Curve { get; set; }

        /// <summary>
        /// RSA 公钥(加密后)
        /// </summary>
        public string PublicKey { get; set; }

        /// <summary>
        /// RSA 私钥(加密后)
        /// </summary>
        public string PrivateKey { get; set; }

        /// <summary>
        /// 对公钥,私钥加密的密钥
        /// </summary>
        public string PassPhrase { get; set; }

        /// <summary>
        /// 对公钥,私钥加密的盐
        /// </summary>
        public string Salt { get; set; }

        /// <summary>
        /// 描述信息
        /// </summary>
        public string Description { get; set; }

    }
}
