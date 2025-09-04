using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace SharpAbp.Abp.CryptoVault
{
    public class SM2Creds : AuditedAggregateRoot<Guid>
    {
        /// <summary>
        /// Unique identifier
        /// </summary>
        public virtual string Identifier { get; set; }

        /// <summary>
        /// Source type
        /// </summary>
        public virtual int SourceType { get; set; }

        /// <summary>
        /// Curve name, default: sm2p256v1 (wapip192v1, sm2p256v1)
        /// </summary>
        public virtual string Curve { get; set; }

        /// <summary>
        /// SM2 public key (encrypted)
        /// </summary>
        public virtual string PublicKey { get; set; }

        /// <summary>
        /// SM2 private key (encrypted)
        /// </summary>
        public virtual string PrivateKey { get; set; }

        /// <summary>
        /// Passphrase used to encrypt the public and private keys
        /// </summary>
        public virtual string PassPhrase { get; set; }

        /// <summary>
        /// Salt used to encrypt the public and private keys
        /// </summary>
        public virtual string Salt { get; set; }

        /// <summary>
        /// Description information
        /// </summary>
        public virtual string Description { get; set; }

        public SM2Creds()
        {

        }

        public SM2Creds(Guid id) : base(id)
        {

        }

        public SM2Creds(
            Guid id,
            string identifier,
            int sourceType,
            string curve,
            string publicKey,
            string privateKey,
            string passPhrase,
            string salt,
            string description) : base(id)
        {
            Identifier = identifier;
            SourceType = sourceType;
            Curve = curve;
            PublicKey = publicKey;
            PrivateKey = privateKey;
            PassPhrase = passPhrase;
            Salt = salt;
            Description = description;
        }
    }
}