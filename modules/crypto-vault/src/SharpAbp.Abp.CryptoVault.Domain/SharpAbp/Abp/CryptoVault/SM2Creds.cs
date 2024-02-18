using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace SharpAbp.Abp.CryptoVault
{
    public class SM2Creds : AuditedAggregateRoot<Guid>
    {
        /// <summary>
        /// 唯一标志
        /// </summary>
        public virtual string Identifier { get; set; }

        /// <summary>
        /// 曲率名称,默认:sm2p256v1  (wapip192v1,sm2p256v1)
        /// </summary>
        public virtual string Curve { get; set; }

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
        public virtual string SecretPassPhrase { get; set; }

        /// <summary>
        /// 对公钥,私钥加密的盐
        /// </summary>
        public virtual string SecretSalt { get; set; }

        /// <summary>
        /// 描述信息
        /// </summary>
        public virtual string Description { get; set; }

        public SM2Creds()
        {

        }

        public SM2Creds(
            Guid id,
            string identifier,
            string curve,
            string publicKey,
            string privateKey,
            string secretPassPhrase,
            string secretSalt,
            string description)
        {
            Id = id;
            Identifier = identifier;
            Curve = curve;
            PublicKey = publicKey;
            PrivateKey = privateKey;
            SecretPassPhrase = secretPassPhrase;
            SecretSalt = secretSalt;
            Description = description;
        }


    }
}
