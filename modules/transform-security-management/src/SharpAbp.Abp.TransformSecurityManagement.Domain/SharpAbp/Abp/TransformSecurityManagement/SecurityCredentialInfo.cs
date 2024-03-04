using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace SharpAbp.Abp.TransformSecurityManagement
{
    public class SecurityCredentialInfo : AuditedAggregateRoot<Guid>
    {
        /// <summary>
        /// 唯一编号
        /// </summary>
        public virtual string Identifier { get; set; }

        /// <summary>
        /// 对应的RSA或者SM2密钥CryptoVault的Id
        /// </summary>
        public virtual Guid CredsId { get; set; }

        /// <summary>
        /// 密钥类型, RSA, SM2
        /// </summary>
        public virtual string KeyType { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        public virtual string BizType { get; set; }

        /// <summary>
        /// 密钥的过期时间
        /// </summary>
        public virtual DateTime? Expires { get; set; }

        /// <summary>
        /// 描述信息
        /// </summary>
        public virtual string Description { get; set; }


        public SecurityCredentialInfo()
        {

        }

        public SecurityCredentialInfo(Guid id)
        {
            Id = id;
        }

        public SecurityCredentialInfo(
            Guid id, 
            string identifier,
            Guid credsId, 
            string keyType, 
            string bizType,
            DateTime? expires,
            string description)
        {
            Id = id;
            Identifier = identifier;
            CredsId = credsId;
            KeyType = keyType;
            BizType = bizType;
            Expires = expires;
            Description = description;
        }
    }
}
