using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace SharpAbp.Abp.CryptoVault
{
    /// <summary>
    /// Represents SM2 cryptographic credentials entity.
    /// </summary>
    public class SM2Creds : AuditedAggregateRoot<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier for the SM2 credentials.
        /// </summary>
        public virtual string Identifier { get; set; }

        /// <summary>
        /// Gets or sets the source type of the credentials.
        /// </summary>
        public virtual int SourceType { get; set; }

        /// <summary>
        /// Gets or sets the elliptic curve name used for SM2 (default: sm2p256v1, options: wapip192v1, sm2p256v1).
        /// </summary>
        public virtual string Curve { get; set; }

        /// <summary>
        /// Gets or sets the SM2 public key in encrypted format.
        /// </summary>
        public virtual string PublicKey { get; set; }

        /// <summary>
        /// Gets or sets the SM2 private key in encrypted format.
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
        /// Initializes a new instance of the <see cref="SM2Creds"/> class.
        /// </summary>
        public SM2Creds()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SM2Creds"/> class with the specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier for the SM2 credentials.</param>
        public SM2Creds(Guid id) : base(id)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SM2Creds"/> class with the specified parameters.
        /// </summary>
        /// <param name="id">The unique identifier for the SM2 credentials.</param>
        /// <param name="identifier">The unique identifier string.</param>
        /// <param name="sourceType">The source type of the credentials.</param>
        /// <param name="curve">The elliptic curve name used for SM2.</param>
        /// <param name="publicKey">The encrypted SM2 public key.</param>
        /// <param name="privateKey">The encrypted SM2 private key.</param>
        /// <param name="passPhrase">The passphrase used for encryption.</param>
        /// <param name="salt">The salt value used in encryption.</param>
        /// <param name="description">The description of the credentials.</param>
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