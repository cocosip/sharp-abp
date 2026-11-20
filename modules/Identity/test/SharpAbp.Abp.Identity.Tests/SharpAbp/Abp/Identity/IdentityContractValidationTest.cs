using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Xunit;

namespace SharpAbp.Abp.Identity;

public class IdentityContractValidationTest
{
    [Theory]
    [InlineData(1, 1, 0)]
    [InlineData(0, 1, 1)]
    [InlineData(1, 0, 1)]
    [InlineData(-1, -1, 2)]
    public void Lockout_Settings_Reject_Nonpositive_Duration_And_Attempts(int duration, int attempts, int errors)
    {
        var settings = new UpdateIdentityLockoutSettingsDto
        {
            LockoutDuration = duration, MaxFailedAccessAttempts = attempts
        };

        Assert.Equal(errors, System.Linq.Enumerable.Count(settings.Validate(new ValidationContext(settings))));
    }

    [Theory]
    [InlineData(3, 1, 0)]
    [InlineData(2, 1, 1)]
    [InlineData(3, 0, 1)]
    [InlineData(0, -1, 2)]
    public void Password_Settings_Reject_Too_Short_Or_Nonpositive_Unique_Characters(int length, int unique, int errors)
    {
        var settings = new UpdateIdentityPasswordSettingsDto { RequiredLength = length, RequiredUniqueChars = unique };

        Assert.Equal(errors, System.Linq.Enumerable.Count(settings.Validate(new ValidationContext(settings))));
    }

    [Fact]
    public void Claim_Equality_Uses_Type_And_Value_And_Handles_Null()
    {
        var comparer = new ClaimEqualityComparer();
        var claim = new Claim("department", "finance");
        var equalClaim = new Claim("department", "finance", ClaimValueTypes.String, "different-issuer");

        Assert.True(comparer.Equals(null, null));
        Assert.False(comparer.Equals(null, claim));
        Assert.False(comparer.Equals(claim, null));
        Assert.True(comparer.Equals(claim, equalClaim));
        Assert.Equal(comparer.GetHashCode(claim), comparer.GetHashCode(equalClaim));
        Assert.False(comparer.Equals(claim, new Claim("department", "engineering")));
        Assert.False(comparer.Equals(claim, new Claim("location", "finance")));
    }
}
