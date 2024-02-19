﻿using System;
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
        public virtual string SecretPassPhrase { get; set; }

        /// <summary>
        /// 对公钥,私钥加密的盐
        /// </summary>
        public virtual string SecretSalt { get; set; }

        /// <summary>
        /// 描述信息
        /// </summary>
        public virtual string Description { get; set; }


        public RSACreds()
        {

        }

        public RSACreds(
            Guid id,
            string identifier,
            int size,
            string publicKey,
            string privateKey,
            string secretPassPhrase,
            string secretSalt,
            string description)
        {
            Id = id;
            Identifier = identifier;
            Size = size;
            PublicKey = publicKey;
            PrivateKey = privateKey;
            SecretPassPhrase = secretPassPhrase;
            SecretSalt = secretSalt;
            Description = description;
        }


    }
}