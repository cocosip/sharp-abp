using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace SharpAbp.MinId;

public class MinIdContractValidationTest
{
    [Theory]
    [InlineData("orders", 10, 1, 0, 0)]
    [InlineData("orders", 10, 1, 9, 0)]
    [InlineData("bad-type", 10, 1, 0, 1)]
    [InlineData("orders", 0, 1, 0, 2)]
    [InlineData("orders", 10, 0, 0, 1)]
    [InlineData("orders", 10, 1, -1, 1)]
    [InlineData("orders", 10, 1, 10, 1)]
    public void Configuration_Create_And_Update_Apply_The_Same_Allocation_Constraints(string bizType, int step, int delta, int remainder, int errors)
    {
        IValidatableObject[] requests =
        [
            new CreateMinIdInfoDto { BizType = bizType, Step = step, Delta = delta, Remainder = remainder },
            new UpdateMinIdInfoDto { BizType = bizType, Step = step, Delta = delta, Remainder = remainder }
        ];

        foreach (var request in requests)
        {
            Assert.Equal(errors, request.Validate(new ValidationContext(request)).Count());
        }
    }

    [Theory]
    [InlineData("orders", "token123", 0)]
    [InlineData("bad-type", "token123", 1)]
    [InlineData("orders", "bad-token", 1)]
    [InlineData("ab", "ab", 2)]
    public void Token_Create_And_Update_Apply_The_Same_Identifier_Constraints(string bizType, string token, int errors)
    {
        IValidatableObject[] requests =
        [
            new CreateMinIdTokenDto { BizType = bizType, Token = token, Remark = "test" },
            new UpdateMinIdTokenDto { BizType = bizType, Token = token, Remark = "test" }
        ];

        foreach (var request in requests)
        {
            Assert.Equal(errors, request.Validate(new ValidationContext(request)).Count());
        }
    }
}
