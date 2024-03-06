using System;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.TransformSecurityManagement
{
    public class SecurityCredentialInfoDto : AuditedEntityDto<Guid>
    {

        /// <summary>
        /// 唯一编号
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// 对应的RSA或者SM2密钥CryptoVault的Id
        /// </summary>
        public Guid CredsId { get; set; }

        /// <summary>
        /// 密钥类型, RSA, SM2
        /// </summary>
        public string KeyType { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        public string BizType { get; set; }

        /// <summary>
        /// 密钥的过期时间
        /// </summary>
        public DateTime? Expires { get; set; }

        /// <summary>
        /// 描述信息
        /// </summary>
        public string Description { get; set; }

    }
}
