using Microsoft.Extensions.Options;
using Volo.Abp.Auditing;
using Xunit;

namespace SharpAbp.Abp.Identity;

public class PasswordAuditingTest
{
    [Fact]
    public void Setting_A_Password_Does_Not_Audit_The_New_Password()
    {
        var input = new SetPasswordDto { NewPassword = "new-password-must-not-be-audited" };
        var serializer = new JsonAuditSerializer(Options.Create(new AbpAuditingOptions()));

        var parameters = serializer.Serialize(new { userId = "target-user", input });

        Assert.DoesNotContain(input.NewPassword, parameters);
        Assert.Contains("target-user", parameters);
    }
}
