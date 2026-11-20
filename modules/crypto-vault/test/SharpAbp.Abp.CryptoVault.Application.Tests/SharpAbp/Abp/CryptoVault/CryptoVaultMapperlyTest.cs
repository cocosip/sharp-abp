using System;
using Xunit;

namespace SharpAbp.Abp.CryptoVault;

public class CryptoVaultMapperlyTest
{
    [Fact]
    public void Rsa_Maps_Key_Metadata_With_Both_Overloads()
    {
        var source = new RSACreds(Guid.NewGuid(), "rsa-key", 2, 3072, "public", "private", "pass", "salt", "description")
        {
            CreationTime = new DateTime(2026, 1, 2), CreatorId = Guid.NewGuid(),
            LastModificationTime = new DateTime(2026, 2, 3), LastModifierId = Guid.NewGuid()
        };
        var mapper = new RSACredsToRSACredsDtoMapper();
        var destination = new RSACredsDto { Identifier = "stale", Size = 1024 };

        var mapped = mapper.Map(source);
        mapper.Map(source, destination);

        foreach (var result in new[] { mapped, destination })
        {
            Assert.Equal(source.Id, result.Id);
            Assert.Equal(source.Identifier, result.Identifier);
            Assert.Equal(source.SourceType, result.SourceType);
            Assert.Equal(source.Size, result.Size);
            Assert.Equal(source.PublicKey, result.PublicKey);
            Assert.Equal(source.PrivateKey, result.PrivateKey);
            Assert.Equal(source.Description, result.Description);
            Assert.Equal(source.CreationTime, result.CreationTime);
            Assert.Equal(source.CreatorId, result.CreatorId);
            Assert.Equal(source.LastModificationTime, result.LastModificationTime);
            Assert.Equal(source.LastModifierId, result.LastModifierId);
        }
    }

    [Fact]
    public void Sm2_Maps_Curve_And_Key_Metadata_With_Both_Overloads()
    {
        var source = new SM2Creds(Guid.NewGuid(), "sm2-key", 1, "sm2p256v1", "public", "private", "pass", "salt", "description");
        var mapper = new SM2CredsToSM2CredsDtoMapper();
        var destination = new SM2CredsDto { Curve = "stale" };

        var mapped = mapper.Map(source);
        mapper.Map(source, destination);

        foreach (var result in new[] { mapped, destination })
        {
            Assert.Equal(source.Id, result.Id);
            Assert.Equal(source.Identifier, result.Identifier);
            Assert.Equal(source.SourceType, result.SourceType);
            Assert.Equal(source.Curve, result.Curve);
            Assert.Equal(source.PublicKey, result.PublicKey);
            Assert.Equal(source.PrivateKey, result.PrivateKey);
            Assert.Equal(source.Description, result.Description);
            Assert.Equal(source.CreationTime, result.CreationTime);
        }
    }
}
