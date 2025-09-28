using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace SharpAbp.Abp.CryptoVault
{
    /// <summary>
    /// Represents RSA cryptographic credentials entity.
    /// </summary>
    public class RSACreds : AuditedAggregateRoot<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier for the RSA credentials.
        /// </summary>
        public virtual string Identifier { get; set; }

        /// <summary>
        /// Gets or sets the source type of the credentials.
        /// </summary>
        public virtual int SourceType { get; set; }

        /// <summary>
        /// Gets or sets the RSA key size in bits (e.g., 1024, 2048, 4096).
        /// </summary>
        public virtual int Size { get; set; }

        /// <summary>
        /// Gets or sets the RSA public key in encrypted format.
        /// </summary>
        public virtual string PublicKey { get; set; }

        /// <summary>
        /// Gets or sets the RSA private key in encrypted format.
        /// </summary>
        public virtual string PrivateKey { get; set; }

        /// <summary>
        /// Gets or sets the passphrase used for encrypting the public and private keys.
        /// </summary>
        public virtual string PassPhrase { get; set; }

        /// <summary>
        /// Gets or sets the salt value used in the encryption process of the keys.
        /// </summary>
        public virtual string Salt { get; set; }

        /// <summary>
        /// Gets or sets the description or additional information about the credentials.
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RSACreds"/> class.
        /// </summary>
        public RSACreds()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RSACreds"/> class with the specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier for the RSA credentials.</param>
        public RSACreds(Guid id) : base(id)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RSACreds"/> class with the specified parameters.
        /// </summary>
        /// <param name="id">The unique identifier for the RSA credentials.</param>
        /// <param name="identifier">The unique identifier string.</param>
        /// <param name="sourceType">The source type of the credentials.</param>
        /// <param name="size">The RSA key size in bits.</param>
        /// <param name="publicKey">The encrypted RSA public key.</param>
        /// <param name="privateKey">The encrypted RSA private key.</param>
        /// <param name="passPhrase">The passphrase used for encryption.</param>
        /// <param name="salt">The salt value used in encryption.</param>
        /// <param name="description">The description of the credentials.</param>
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