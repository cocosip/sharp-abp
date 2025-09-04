using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace SharpAbp.Abp.TransformSecurityManagement
{
    public class SecurityCredentialInfo : AuditedAggregateRoot<Guid>
    {
        /// <summary>
        /// Unique identifier
        /// </summary>
        public virtual string Identifier { get; set; }

        /// <summary>
        /// Id of the corresponding RSA or SM2 key CryptoVault
        /// </summary>
        public virtual Guid CredsId { get; set; }

        /// <summary>
        /// Key type, RSA or SM2
        /// </summary>
        public virtual string KeyType { get; set; }

        /// <summary>
        /// Business type
        /// </summary>
        public virtual string BizType { get; set; }

        /// <summary>
        /// Key expiration time
        /// </summary>
        public virtual DateTime? Expires { get; set; }

        /// <summary>
        /// Description information
        /// </summary>
        public virtual string Description { get; set; }


        public SecurityCredentialInfo()
        {

        }

        public SecurityCredentialInfo(Guid id) : base(id)
        {
        }

        public SecurityCredentialInfo(
            Guid id,
            string identifier,
            Guid credsId,
            string keyType,
            string bizType,
            DateTime? expires,
            string description) : base(id)
        {
            Identifier = identifier;
            CredsId = credsId;
            KeyType = keyType;
            BizType = bizType;
            Expires = expires;
            Description = description;
        }
    }
}