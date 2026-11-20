using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Identity;
using Xunit;

namespace SharpAbp.Abp.Identity;

public class IdentityAssignmentsTest : IdentityApplicationTestBase
{
    [Fact]
    public async Task Role_Crud_Queries_And_User_Assignment_Are_Persisted()
    {
        var roles = new IdentityRoleController(GetRequiredService<IIdentityRoleAppService>());
        var users = new IdentityUserController(GetRequiredService<IIdentityUserAppService>());
        var role = await roles.CreateAsync(new IdentityRoleCreateDto { Name = "auditor", IsPublic = true });
        Assert.Equal(role.Id, (await roles.GetAsync(role.Id)).Id);
        Assert.Equal(role.Id, Assert.Single((await roles.GetAllListAsync()).Items).Id);
        var page = await roles.GetListAsync(new GetIdentityRolesInput { Filter = "auditor" });
        Assert.Equal(1, page.TotalCount);
        Assert.Equal(role.Id, Assert.Single(page.Items).Id);

        role = await roles.UpdateAsync(role.Id, new IdentityRoleUpdateDto { Name = "reviewer", IsPublic = false });
        Assert.Equal("reviewer", role.Name);
        Assert.False(role.IsPublic);
        var user = await users.CreateAsync(new NewIdentityUserCreateDto
        {
            UserName = "assigned-user", Email = "assigned@example.com", Password = "1q2w3E*"
        });
        await users.UpdateRolesAsync(user.Id, new IdentityUserUpdateRolesDto { RoleNames = ["reviewer"] });
        Assert.Equal(role.Id, Assert.Single((await users.GetRolesAsync(user.Id)).Items).Id);
        await users.UpdateRolesAsync(user.Id, new IdentityUserUpdateRolesDto { RoleNames = [] });
        Assert.Empty((await users.GetRolesAsync(user.Id)).Items);
        await roles.DeleteAsync(role.Id);
        await Assert.ThrowsAnyAsync<EntityNotFoundException>(() => roles.GetAsync(role.Id));
    }

    [Fact]
    public async Task Organization_Membership_Queries_Track_Additions_And_Removals()
    {
        var organizations = new OrganizationUnitController(GetRequiredService<IOrganizationUnitAppService>());
        var roles = new IdentityRoleController(GetRequiredService<IIdentityRoleAppService>());
        var users = new IdentityUserController(GetRequiredService<IIdentityUserAppService>());
        var organizationId = await organizations.CreateAsync(new CreateOrganizationUnitDto { DisplayName = "Finance" });
        var role = await roles.CreateAsync(new IdentityRoleCreateDto { Name = "finance-role" });
        var user = await users.CreateAsync(new NewIdentityUserCreateDto
        {
            UserName = "finance-user", Email = "finance@example.com", Password = "1q2w3E*"
        });

        Assert.Equal(organizationId, Assert.Single(await organizations.GetListAsync([organizationId])).Id);
        Assert.Equal(role.Id, Assert.Single((await organizations.GetUnaddedRolesPagedListAsync(organizationId,
            new OrganizationUnitUnaddedRolePagedRequestDto())).Items).Id);
        Assert.Equal(user.Id, Assert.Single((await organizations.GetUnaddedMembersPagedListAsync(organizationId,
            new OrganizationUnitUnaddedMemberPagedRequestDto())).Items).Id);
        await organizations.AddRoleToOrganizationUnitAsync(organizationId, new AddRoleToOrganizationUnitDto { RoleIds = [role.Id] });
        await organizations.AddMemberToOrganizationUnitAsync(organizationId, new AddMemberToOrganizationUnitDto { UserIds = [user.Id] });

        Assert.Equal(role.Id, Assert.Single((await organizations.GetRolesPagedListAsync(organizationId,
            new OrganizationUnitRolePagedRequestDto())).Items).Id);
        Assert.Equal(user.Id, Assert.Single((await organizations.GetMembersPagedListAsync(organizationId,
            new OrganizationUnitMemberPagedRequestDto())).Items).Id);
        Assert.Equal(organizationId, Assert.Single(await users.GetOrganizationUnitsAsync(user.Id)).Id);
        await organizations.RemoveRoleFromOrganizationUnitAsync(organizationId, role.Id);
        await organizations.RemoveMemberFromOrganizationUnitAsync(organizationId, user.Id);
        Assert.Empty((await organizations.GetRolesPagedListAsync(organizationId, new OrganizationUnitRolePagedRequestDto())).Items);
        Assert.Empty((await organizations.GetMembersPagedListAsync(organizationId, new OrganizationUnitMemberPagedRequestDto())).Items);
    }
}
