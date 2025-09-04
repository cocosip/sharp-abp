    using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace SharpAbp.Abp.CryptoVault
{
    public class RSACreds : AuditedAggregateRoot<Guid>
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
        /// RSA key size (1024, 2048)
        /// </summary>
        public virtual int Size { get; set; }

        /// <summary>
        /// RSA public key (encrypted)
        /// </summary>
        public virtual string PublicKey { get; set; }

        /// <summary>
        /// RSA private key (encrypted)
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


        public RSACreds()
        {

        }

        public RSACreds(Guid id) : base(id)
        {

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
            string description) : base(id)
        {
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