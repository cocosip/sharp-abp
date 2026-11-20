using System;
using Xunit;

namespace SharpAbp.Abp.TransformSecurityManagement;

public class SecurityCredentialMapperlyTest
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Mapping_Preserves_Key_Reference_And_Optional_Expiry(bool expires)
    {
        var source = new SecurityCredentialInfo(Guid.NewGuid(), "credential", Guid.NewGuid(), "RSA", "Login",
            expires ? new DateTime(2027, 1, 2, 3, 4, 5, DateTimeKind.Utc) : null, "description")
        {
            CreationTime = new DateTime(2026, 1, 2), CreatorId = Guid.NewGuid()
        };
        var mapper = new SecurityCredentialInfoToSecurityCredentialInfoDtoMapper();
        var destination = new SecurityCredentialInfoDto { Expires = DateTime.MaxValue, Identifier = "stale" };

        var mapped = mapper.Map(source);
        mapper.Map(source, destination);

        foreach (var result in new[] { mapped, destination })
        {
            Assert.Equal(source.Id, result.Id);
            Assert.Equal(source.Identifier, result.Identifier);
            Assert.Equal(source.CredsId, result.CredsId);
            Assert.Equal(source.KeyType, result.KeyType);
            Assert.Equal(source.BizType, result.BizType);
            Assert.Equal(source.Expires, result.Expires);
            Assert.Equal(source.Description, result.Description);
            Assert.Equal(source.CreationTime, result.CreationTime);
            Assert.Equal(source.CreatorId, result.CreatorId);
        }
    }
}
