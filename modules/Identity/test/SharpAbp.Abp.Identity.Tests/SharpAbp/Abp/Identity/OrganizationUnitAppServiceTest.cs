using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Identity;
using Xunit;

namespace SharpAbp.Abp.Identity
{
    public class OrganizationUnitAppServiceTest : IdentityApplicationTestBase
    {
        private readonly IOrganizationUnitAppService _organizationUnitAppService;
        public OrganizationUnitAppServiceTest()
        {
            _organizationUnitAppService = GetRequiredService<IOrganizationUnitAppService>();
        }

        [Fact]
        public async Task OrganizationUnit_Crud_TestAsync()
        {
            var rootOu1Id = await _organizationUnitAppService.CreateAsync(new CreateOrganizationUnitDto()
            {
                DisplayName = "RootOu1"
            });

            var rootOu2Id = await _organizationUnitAppService.CreateAsync(new CreateOrganizationUnitDto()
            {
                DisplayName = "RootOu2"
            });


            var ou1Id = await _organizationUnitAppService.CreateAsync(new CreateOrganizationUnitDto()
            {
                ParentId = rootOu1Id,
                DisplayName = "Ou1"
            });

            var ou2Id = await _organizationUnitAppService.CreateAsync(new CreateOrganizationUnitDto()
            {
                ParentId = rootOu1Id,
                DisplayName = "Ou2",
            });

            var ou3Id = await _organizationUnitAppService.CreateAsync(new CreateOrganizationUnitDto()
            {
                ParentId = rootOu2Id,
                DisplayName = "Ou3"
            });


            var ou1 = await _organizationUnitAppService.GetAsync(ou1Id);
            Assert.Equal("Ou1", ou1.DisplayName);

            var childrenOu1 = await _organizationUnitAppService.GetChildrenAsync(rootOu1Id);
            Assert.Equal(2, childrenOu1.Count);

            await _organizationUnitAppService.UpdateAsync(ou2Id, new UpdateOrganizationUnitDto()
            {
                DisplayName = "Ou22"
            });

            var ou22 = await _organizationUnitAppService.GetAsync(ou2Id);
            Assert.Equal("Ou22", ou22.DisplayName);

            await _organizationUnitAppService.MoveAsync(ou1Id, new MoveOrganizationUnitDto()
            {
                NewParentId = rootOu2Id
            });


            var childrenOu2 = await _organizationUnitAppService.GetChildrenAsync(rootOu2Id);
            Assert.Equal(2, childrenOu2.Count);

            var childrenOu3 = await _organizationUnitAppService.GetChildrenAsync(rootOu1Id);
            Assert.Single(childrenOu3);

            var allOus = await _organizationUnitAppService.GetAllAsync();
            Assert.Equal(5, allOus.Count);

            var ou22_2 = await _organizationUnitAppService.FindByDisplayNameAsync("Ou22");
            Assert.Equal(ou2Id, ou22_2.Id);

            var pagedOus = await _organizationUnitAppService.GetPagedListAsync(new OrganizationUnitPagedRequestDto());

            Assert.Equal(5, pagedOus.TotalCount);
            Assert.Equal(5, pagedOus.Items.Count);


            await _organizationUnitAppService.DeleteAsync(ou2Id);

            await Assert.ThrowsAsync<EntityNotFoundException<OrganizationUnit>>(() =>
            {
                return _organizationUnitAppService.GetAsync(ou2Id);
            });
        }

    }
}
