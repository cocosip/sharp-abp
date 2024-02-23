using SharpAbp.Abp.Crypto.RSA;
using SharpAbp.Abp.Crypto.SM2;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Security.Encryption;


namespace SharpAbp.Abp.CryptoVault
{
    public class DefaultKeyService : IKeyService, ITransientDependency
    {
        protected IGuidGenerator GuidGenerator { get; }
        protected IStringEncryptionService StringEncryptionService { get; }
        protected IRSAEncryptionService RSAEncryptionService { get; }
        protected ISm2EncryptionService Sm2EncryptionService { get; }
        public DefaultKeyService(
            IGuidGenerator guidGenerator,
            IStringEncryptionService stringEncryptionService,
            IRSAEncryptionService rsaEncryptionService,
            ISm2EncryptionService sm2EncryptionService)
        {
            GuidGenerator = guidGenerator;
            StringEncryptionService = stringEncryptionService;
            RSAEncryptionService = rsaEncryptionService;
            Sm2EncryptionService = sm2EncryptionService;
        }

        public virtual List<RSACreds> GenerateRSACreds(int size, int count)
        {
            var rsaCredsList = new List<RSACreds>();
            for (int i = 0; i < count; i++)
            {
                var keyPair = RSAEncryptionService.GenerateRSAKeyPair(size);
                var pub = RSAExtensions.ExportPublicKey(keyPair.Public);
                var priv = RSAExtensions.ExportPrivateKey(keyPair.Private);

                var passPhrase = GeneratePassPhrase();
                var salt = GenerateSalt();

                var rsaCreds = new RSACreds(GuidGenerator.Create())
                {
                    Identifier = GenerateIdentifier(),
                    Size = size,
                    SourceType = (int)KeySourceType.Generate,
                    PublicKey = EncryptKey(pub, passPhrase, salt),
                    PrivateKey = EncryptKey(priv, passPhrase, salt),
                    PassPhrase = passPhrase,
                    Salt = salt
                };

                rsaCredsList.Add(rsaCreds);
            }
            return rsaCredsList;
        }


        public virtual List<SM2Creds> GenerateSM2Creds(string curve, int count)
        {
            var sm2CredsList = new List<SM2Creds>();
            for (int i = 0; i < count; i++)
            {
                var keyPair = Sm2EncryptionService.GenerateKeyPair(curve);
                var pub = Sm2Extensions.ExportPublicKey(keyPair.Public);
                var priv = Sm2Extensions.ExportPrivateKey(keyPair.Private);

                var passPhrase = GeneratePassPhrase();
                var salt = GenerateSalt();

                var sm2Creds = new SM2Creds(GuidGenerator.Create())
                {
                    Identifier = GenerateIdentifier(),
                    SourceType = (int)KeySourceType.Generate,
                    Curve = curve,
                    PublicKey = EncryptKey(pub, passPhrase, salt),
                    PrivateKey = EncryptKey(priv, passPhrase, salt),
                    PassPhrase = passPhrase,
                    Salt = salt
                };



                sm2CredsList.Add(sm2Creds);
            }
            return sm2CredsList;
        }




        public virtual string EncryptKey(string plainText, string passPhrase, string salt)
        {
            return StringEncryptionService.Encrypt(plainText, passPhrase, Encoding.UTF8.GetBytes(salt));
        }

        public virtual string DecryptKey(string plainText, string passPhrase, string salt)
        {
            return StringEncryptionService.Decrypt(plainText, passPhrase, Encoding.UTF8.GetBytes(salt));
        }

        public virtual string GenerateIdentifier()
        {
            return GuidGenerator.Create().ToString("N");
        }

        public virtual string GeneratePassPhrase()
        {
            return GenerateRandomString(12);
        }

        public virtual string GenerateSalt()
        {
            return GenerateRandomString(8);
        }

        protected virtual string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            char[] randomArray = new char[length];
            for (int i = 0; i < length; i++)
            {
                randomArray[i] = chars[random.Next(chars.Length)];
            }
            return new string(randomArray);
        }



    }
}
