using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace SharpAbp.Abp.CryptoVault;

public class CryptoVaultContractValidationTest
{
    [Theory]
    [InlineData(1024, 1, 0)]
    [InlineData(2048, 100, 0)]
    [InlineData(3072, 1, 0)]
    [InlineData(4096, 1, 0)]
    [InlineData(512, 1, 1)]
    [InlineData(2048, 0, 1)]
    [InlineData(2048, 101, 1)]
    [InlineData(0, 0, 2)]
    public void Rsa_Generation_Validates_Key_Size_And_Batch_Limits(int size, int count, int errors)
    {
        var request = new GenerateRSACredsDto { Size = size, Count = count };

        var results = request.Validate(new ValidationContext(request)).ToList();

        Assert.Equal(errors, results.Count);
        Assert.All(results, result => Assert.Single(result.MemberNames));
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(100, 0)]
    [InlineData(0, 1)]
    [InlineData(101, 1)]
    public void Sm2_Generation_Enforces_Batch_Limits(int count, int errors)
    {
        var request = new GenerateSM2CredsDto { Curve = "sm2p256v1", Count = count };

        var results = request.Validate(new ValidationContext(request)).ToList();

        Assert.Equal(errors, results.Count);
        Assert.All(results, result => Assert.Equal(nameof(request.Count), Assert.Single(result.MemberNames)));
    }
}
