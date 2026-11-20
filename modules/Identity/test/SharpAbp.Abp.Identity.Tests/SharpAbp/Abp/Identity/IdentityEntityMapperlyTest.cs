using System;
using Volo.Abp.Data;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.ObjectMapping;
using Volo.Abp.SecurityLog;
using Xunit;

namespace SharpAbp.Abp.Identity;

public class IdentityEntityMapperlyTest : IdentityApplicationTestBase
{
    private readonly IObjectMapper<IdentityApplicationModule> _mapper;
    private const string PublicProperty = "IdentityMapperPublic";
    private const string PrivateProperty = "IdentityMapperPrivate";

    public IdentityEntityMapperlyTest()
    {
        _mapper = GetRequiredService<IObjectMapper<IdentityApplicationModule>>();
        ObjectExtensionManager.Instance.AddOrUpdateProperty<IdentityClaimType, string>(PublicProperty);
        ObjectExtensionManager.Instance.AddOrUpdateProperty<IdentityClaimTypeDto, string>(PublicProperty);
        ObjectExtensionManager.Instance.AddOrUpdateProperty<OrganizationUnit, string>(PublicProperty);
        ObjectExtensionManager.Instance.AddOrUpdateProperty<OrganizationUnitDto, string>(PublicProperty);
        ObjectExtensionManager.Instance.AddOrUpdateProperty<IdentitySecurityLog, string>(PublicProperty);
        ObjectExtensionManager.Instance.AddOrUpdateProperty<IdentitySecurityLogDto, string>(PublicProperty);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Claim_Type_Maps_Enum_Display_And_Only_Defined_Extra_Properties(bool existing)
    {
        var source = new IdentityClaimType(Guid.NewGuid(), "department", true, false, "[a-z]+", "letters", "description", IdentityClaimValueType.String);
        source.SetProperty(PublicProperty, "public");
        source.SetProperty(PrivateProperty, "private");

        var result = existing ? _mapper.Map(source, new IdentityClaimTypeDto()) : _mapper.Map<IdentityClaimType, IdentityClaimTypeDto>(source);

        Assert.Equal(source.Id, result.Id);
        Assert.Equal(source.Name, result.Name);
        Assert.Equal(source.Required, result.Required);
        Assert.Equal(source.IsStatic, result.IsStatic);
        Assert.Equal(source.Regex, result.Regex);
        Assert.Equal(source.RegexDescription, result.RegexDescription);
        Assert.Equal(source.Description, result.Description);
        Assert.Equal(source.ValueType, result.ValueType);
        Assert.Equal("String", result.ValueTypeAsString);
        Assert.Equal("public", result.ExtraProperties[PublicProperty]);
        Assert.False(result.ExtraProperties.ContainsKey(PrivateProperty));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Organization_Maps_Parent_And_Defined_Extra_Properties(bool existing)
    {
        var source = new OrganizationUnit(Guid.NewGuid(), "Finance", Guid.NewGuid(), Guid.NewGuid());
        source.SetProperty(PublicProperty, "public");
        source.SetProperty(PrivateProperty, "private");

        var result = existing ? _mapper.Map(source, new OrganizationUnitDto()) : _mapper.Map<OrganizationUnit, OrganizationUnitDto>(source);

        Assert.Equal(source.Id, result.Id);
        Assert.Equal(source.TenantId, result.TenantId);
        Assert.Equal(source.ParentId, result.ParentId);
        Assert.Equal(source.DisplayName, result.DisplayName);
        Assert.Equal("public", result.ExtraProperties[PublicProperty]);
        Assert.False(result.ExtraProperties.ContainsKey(PrivateProperty));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Security_Log_Maps_Audit_Context_And_Defined_Extra_Properties(bool existing)
    {
        var source = new IdentitySecurityLog(SimpleGuidGenerator.Instance, new SecurityLogInfo
        {
            ApplicationName = "app", Identity = "identity", Action = "Login", UserId = Guid.NewGuid(),
            UserName = "user", TenantId = Guid.NewGuid(), ClientId = "client", CorrelationId = "correlation"
        });
        source.SetProperty(PublicProperty, "public");
        source.SetProperty(PrivateProperty, "private");

        var result = existing ? _mapper.Map(source, new IdentitySecurityLogDto()) : _mapper.Map<IdentitySecurityLog, IdentitySecurityLogDto>(source);

        Assert.Equal(source.Id, result.Id);
        Assert.Equal(source.ApplicationName, result.ApplicationName);
        Assert.Equal(source.Identity, result.Identity);
        Assert.Equal(source.Action, result.Action);
        Assert.Equal(source.UserId, result.UserId);
        Assert.Equal(source.UserName, result.UserName);
        Assert.Equal(source.TenantId, result.TenantId);
        Assert.Equal(source.ClientId, result.ClientId);
        Assert.Equal(source.CorrelationId, result.CorrelationId);
        Assert.Equal("public", result.ExtraProperties[PublicProperty]);
        Assert.False(result.ExtraProperties.ContainsKey(PrivateProperty));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Claims_And_Organization_Roles_Map_Owner_Identifiers(bool existing)
    {
        var tenantId = Guid.NewGuid();
        var userClaim = new IdentityUserClaim(Guid.NewGuid(), Guid.NewGuid(), "department", "finance", tenantId);
        var roleClaim = new IdentityRoleClaim(Guid.NewGuid(), Guid.NewGuid(), "scope", "read", tenantId);
        var role = new OrganizationUnitRole(Guid.NewGuid(), Guid.NewGuid(), tenantId);

        var userResult = existing ? _mapper.Map(userClaim, new IdentityUserClaimDto()) : _mapper.Map<IdentityUserClaim, IdentityUserClaimDto>(userClaim);
        var roleResult = existing ? _mapper.Map(roleClaim, new IdentityRoleClaimDto()) : _mapper.Map<IdentityRoleClaim, IdentityRoleClaimDto>(roleClaim);
        var organizationResult = existing ? _mapper.Map(role, new OrganizationUnitRoleDto()) : _mapper.Map<OrganizationUnitRole, OrganizationUnitRoleDto>(role);

        Assert.Equal(userClaim.Id, userResult.Id);
        Assert.Equal(userClaim.UserId, userResult.UserId);
        Assert.Equal(userClaim.ClaimType, userResult.ClaimType);
        Assert.Equal(userClaim.ClaimValue, userResult.ClaimValue);
        Assert.Equal(roleClaim.Id, roleResult.Id);
        Assert.Equal(roleClaim.RoleId, roleResult.RoleId);
        Assert.Equal(roleClaim.ClaimType, roleResult.ClaimType);
        Assert.Equal(roleClaim.ClaimValue, roleResult.ClaimValue);
        Assert.Equal(role.RoleId, organizationResult.RoleId);
        Assert.Equal(role.OrganizationUnitId, organizationResult.OrganizationUnitId);
        Assert.Equal(tenantId, organizationResult.TenantId);
    }
}
