using Microsoft.Extensions.Options;
using Volo.Abp.Auditing;
using Xunit;

namespace SharpAbp.Abp.CryptoVault;

public class CredentialAuditingTest
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Credential_Creation_Audits_Public_Metadata_Without_Private_Keys(bool rsa)
    {
        const string privateKey = "private-key-must-not-be-audited";
        object input = rsa
            ? new CreateRSACredsDto { Size = 2048, PublicKey = "public-key", PrivateKey = privateKey }
            : new CreateSM2CredsDto { Curve = "sm2p256v1", PublicKey = "public-key", PrivateKey = privateKey };
        var serializer = new JsonAuditSerializer(Options.Create(new AbpAuditingOptions()));

        var parameters = serializer.Serialize(new { input });

        Assert.DoesNotContain(privateKey, parameters);
        Assert.Contains("public-key", parameters);
        Assert.Contains(rsa ? "2048" : "sm2p256v1", parameters);
    }
}
