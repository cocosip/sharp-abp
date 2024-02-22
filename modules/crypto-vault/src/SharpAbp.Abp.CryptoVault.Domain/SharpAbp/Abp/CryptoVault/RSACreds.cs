using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace SharpAbp.Abp.CryptoVault
{
    public class RSACreds : AuditedAggregateRoot<Guid>
    {
        /// <summary>
        /// 唯一标志
        /// </summary>
        public virtual string Identifier { get; set; }

        /// <summary>
        /// SourceType
        /// </summary>
        public virtual int SourceType { get; set; }

        /// <summary>
        /// RSA密钥长度 (1024,2048)
        /// </summary>
        public virtual int Size { get; set; }

        /// <summary>
        /// RSA 公钥(加密后)
        /// </summary>
        public virtual string PublicKey { get; set; }

        /// <summary>
        /// RSA 私钥(加密后)
        /// </summary>
        public virtual string PrivateKey { get; set; }

        /// <summary>
        /// 对公钥,私钥加密的密钥
        /// </summary>
        public virtual string PassPhrase { get; set; }

        /// <summary>
        /// 对公钥,私钥加密的盐
        /// </summary>
        public virtual string Salt { get; set; }

        /// <summary>
        /// 描述信息
        /// </summary>
        public virtual string Description { get; set; }


        public RSACreds()
        {

        }

        public RSACreds(Guid id)
        {
            Id = id;
        }

        public RSACreds(
            Guid id,
            string identifier,
            int sourceType,
            int size,
            string publicKey,
            string privateKey,
            string passPhrase,
            string salt,
            string description)
        {
            Id = id;
            Identifier = identifier;
            SourceType = sourceType;
            Size = size;
            PublicKey = publicKey;
            PrivateKey = privateKey;
            PassPhrase = passPhrase;
            Salt = salt;
            Description = description;
        }
    }
}
