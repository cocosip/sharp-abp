using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.Extensions.Options;
using Volo.Abp.Auditing;
using Xunit;

namespace SharpAbp.MinId;

public class MinIdTokenAuditingTest : MinIdApplicationTestBase
{
    [Theory]
    [InlineData("create")]
    [InlineData("update")]
    [InlineData("filter")]
    [InlineData("find")]
    public void Token_Dtos_Audit_Business_Metadata_Without_Secrets(string operation)
    {
        const string token = "token-must-not-be-audited";
        object input = operation switch
        {
            "create" => new CreateMinIdTokenDto { BizType = "orders", Token = token, Remark = "administrative metadata" },
            "update" => new UpdateMinIdTokenDto { BizType = "orders", Token = token, Remark = "administrative metadata" },
            "find" => new FindMinIdTokenDto { BizType = "orders", Token = token },
            _ => new MinIdTokenPagedRequestDto { BizType = "orders", Token = token }
        };
        var serializer = new JsonAuditSerializer(Options.Create(new AbpAuditingOptions()));

        var parameters = serializer.Serialize(new { input });

        Assert.DoesNotContain(token, parameters);
        Assert.Contains("orders", parameters);
        if (operation == "create" || operation == "update")
        {
            Assert.Contains("administrative metadata", parameters);
        }
    }

    [Theory]
    [InlineData(typeof(IdController), nameof(IdController.SimpleNextIdAsync))]
    [InlineData(typeof(IdController), nameof(IdController.NextIdAsync))]
    [InlineData(typeof(IdController), nameof(IdController.SimpleBatchNextIdAsync))]
    [InlineData(typeof(IdController), nameof(IdController.BatchNextIdAsync))]
    [InlineData(typeof(IdController), nameof(IdController.GetNextSegmentIdAsync))]
    [InlineData(typeof(MinIdTokenController), nameof(MinIdTokenController.FindByTokenAsync))]
    [InlineData(typeof(MinIdTokenAppService), nameof(MinIdTokenAppService.FindByTokenAsync))]
    [InlineData(typeof(IMinIdTokenAppService), nameof(IMinIdTokenAppService.FindByTokenAsync))]
    public void Token_Methods_Keep_Audit_Actions_Without_Secret_Arguments(Type serviceType, string methodName)
    {
        const string token = "primitive-token-must-not-be-audited";
        var method = serviceType.GetMethod(methodName);
        var arguments = method.GetParameters().Length == 3
            ? new object[] { "orders", token, 12 }
            : new object[] { "orders", token };
        var helper = GetRequiredService<IAuditingHelper>();

        Assert.True(helper.ShouldSaveAudit(method, defaultValue: true));
        var action = helper.CreateAuditLogAction(new AuditLogInfo(), serviceType, method, arguments);

        Assert.Equal(methodName, action.MethodName);
        Assert.Contains("orders", action.Parameters);
        Assert.DoesNotContain(token, action.Parameters);
        if (arguments.Length == 3)
        {
            Assert.Contains("12", action.Parameters);
        }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Invalid_Token_Validation_Does_Not_Expose_The_Submitted_Secret(bool create)
    {
        const string token = "invalid-token-secret!";
        IValidatableObject input = create
            ? new CreateMinIdTokenDto { BizType = "orders", Token = token, Remark = "test" }
            : new UpdateMinIdTokenDto { BizType = "orders", Token = token, Remark = "test" };

        var errors = input.Validate(new ValidationContext(input)).ToList();

        Assert.NotEmpty(errors);
        Assert.All(errors, error => Assert.DoesNotContain(token, error.ErrorMessage));
        Assert.Contains(errors, error => error.ErrorMessage.Contains("Token"));
    }
}
