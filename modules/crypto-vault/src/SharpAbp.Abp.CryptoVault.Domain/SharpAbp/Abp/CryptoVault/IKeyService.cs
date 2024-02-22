using System.Collections.Generic;

namespace SharpAbp.Abp.CryptoVault
{
    public interface IKeyService
    {
        string EncryptKey(string plainText, string passPhrase, string salt);
        string DecryptKey(string cipherText, string passPhrase, string salt);

        List<RSACreds> GenerateRSACreds(int size, int count);
        List<SM2Creds> GenerateSM2Creds(string curve, int count);
        string GenerateIdentifier();
        string GeneratePassPhrase();
        string GenerateSalt();

    }
}
