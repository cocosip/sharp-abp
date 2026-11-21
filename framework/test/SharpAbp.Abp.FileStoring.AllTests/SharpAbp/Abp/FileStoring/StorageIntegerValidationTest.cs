using SharpAbp.Abp.Validation;
using Volo.Abp.Validation;
using Xunit;

namespace SharpAbp.Abp.FileStoring;

public class StorageIntegerValidationTest
{
    [Theory]
    [InlineData("2147483647", true)]
    [InlineData("-2147483648", true)]
    [InlineData("+1", true)]
    [InlineData("2147483648", false)]
    [InlineData("-2147483649", false)]
    [InlineData("1 ", false)]
    [InlineData("1\0", false)]
    public void ShouldInt_Requires_A_Representable_Int32(string value, bool valid)
    {
        var result = new AbpValidationResult();

        ValidateHelper.ShouldInt(result, "Storage", "Setting", value);

        Assert.Equal(valid, result.Errors.Count == 0);
    }
}
