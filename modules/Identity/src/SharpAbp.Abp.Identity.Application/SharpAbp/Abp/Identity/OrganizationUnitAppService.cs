using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Identity;

namespace SharpAbp.Abp.Identity
{
    [Authorize(IdentityPermissions.OrganizationUnits.Default)]
    public class OrganizationUnitAppService : IdentityAppServiceBase, IOrganizationUnitAppService
    {
        protected IOrganizationUnitRepository OrganizationUnitRepository { get; }
        protected OrganizationUnitManager OrganizationUnitManager { get; }
        protected IdentityUserManager IdentityUserManager { get; }
        public OrganizationUnitAppService(
            IOrganizationUnitRepository organizationUnitRepository,
            OrganizationUnitManager organizationUnitManager,
            IdentityUserManager identityUserManager)
        {
            OrganizationUnitRepository = organizationUnitRepository;
            OrganizationUnitManager = organizationUnitManager;
            IdentityUserManager = identityUserManager;
        }

        /// <summary>
        /// Get by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(IdentityPermissions.OrganizationUnits.Default)]
        public virtual async Task<OrganizationUnitDto> GetAsync(Guid id)
        {
            var organizationUnit = await OrganizationUnitRepository.GetAsync(id);
            return ObjectMapper.Map<OrganizationUnit, OrganizationUnitDto>(organizationUnit);
        }

        /// <summary>
        /// Get all
        /// </summary>
        /// <returns></returns>
        [Authorize(IdentityPermissions.OrganizationUnits.Default)]
        public virtual async Task<List<OrganizationUnitDto>> GetAllAsync()
        {
            var organizationUnits = await OrganizationUnitRepository.GetListAsync(false, default);
            return ObjectMapper.Map<List<OrganizationUnit>, List<OrganizationUnitDto>>(organizationUnits);
        }

        /// <summary>
        /// Get paged list
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(IdentityPermissions.OrganizationUnits.Default)]
        public virtual async Task<PagedResultDto<OrganizationUnitDto>> GetPagedListAsync(OrganizationUnitPagedRequestDto input)
        {
            var count = await OrganizationUnitRepository.GetCountAsync();
            var organizationUnits = await OrganizationUnitRepository.GetListAsync(
                input.Sorting,
                input.MaxResultCount,
                input.SkipCount);

            return new PagedResultDto<OrganizationUnitDto>(
              count,
              ObjectMapper.Map<List<OrganizationUnit>, List<OrganizationUnitDto>>(organizationUnits)
              );
        }

        /// <summary>
        /// Find by displayName
        /// </summary>
        /// <param name="displayName"></param>
        /// <returns></returns>
        [Authorize(IdentityPermissions.OrganizationUnits.Default)]
        public virtual async Task<OrganizationUnitDto> FindByDisplayNameAsync(string displayName)
        {
            var organizationUnit = await OrganizationUnitRepository.GetAsync(displayName);
            return ObjectMapper.Map<OrganizationUnit, OrganizationUnitDto>(organizationUnit);
        }

        /// <summary>
        /// Get children by pateintId
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        [Authorize(IdentityPermissions.OrganizationUnits.Default)]
        public virtual async Task<List<OrganizationUnitDto>> GetChildrenAsync(Guid? parentId)
        {
            var organizationUnits = await OrganizationUnitRepository.GetChildrenAsync(parentId);
            return ObjectMapper.Map<List<OrganizationUnit>, List<OrganizationUnitDto>>(organizationUnits);
        }

        /// <summary>
        /// GetAllChildrenWithParentCodeAsync
        /// </summary>
        /// <param name="code"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        [Authorize(IdentityPermissions.OrganizationUnits.Default)]
        public virtual async Task<List<OrganizationUnitDto>> GetAllChildrenWithParentCodeAsync(string code, Guid? parentId)
        {
            var organizationUnits = await OrganizationUnitRepository.GetAllChildrenWithParentCodeAsync(code, parentId);
            return ObjectMapper.Map<List<OrganizationUnit>, List<OrganizationUnitDto>>(organizationUnits);
        }

        /// <summary>
        /// Get list by ids
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [Authorize(IdentityPermissions.OrganizationUnits.Default)]
        public virtual async Task<List<OrganizationUnitDto>> GetListAsync(List<Guid> ids)
        {
            var organizationUnits = await OrganizationUnitRepository.GetListAsync(ids);
            return ObjectMapper.Map<List<OrganizationUnit>, List<OrganizationUnitDto>>(organizationUnits);
        }

        /// <summary>
        /// Get roles paged list
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(IdentityPermissions.OrganizationUnits.Default)]
        public virtual async Task<PagedResultDto<IdentityRoleDto>> GetRolesPagedListAsync(Guid id, OrganizationUnitRolePagedRequestDto input)
        {
            var organizationUnit = await OrganizationUnitRepository.GetAsync(id);

            var count = await OrganizationUnitRepository.GetRolesCountAsync(organizationUnit);
            var identityRoles = await OrganizationUnitRepository.GetRolesAsync(
                organizationUnit,
                input.Sorting,
                input.MaxResultCount,
                input.SkipCount);

            return new PagedResultDto<IdentityRoleDto>(
              count,
              ObjectMapper.Map<List<IdentityRole>, List<IdentityRoleDto>>(identityRoles)
              );
        }

        /// <summary>
        /// Get unAdded roles paged list
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(IdentityPermissions.OrganizationUnits.Default)]
        public virtual async Task<PagedResultDto<IdentityRoleDto>> GetUnaddedRolesPagedListAsync(Guid id, OrganizationUnitUnaddedRolePagedRequestDto input)
        {
            var organizationUnit = await OrganizationUnitRepository.GetAsync(id);

            var count = await OrganizationUnitRepository.GetUnaddedRolesCountAsync(organizationUnit, input.Filter);
            var identityRoles = await OrganizationUnitRepository.GetUnaddedRolesAsync(
                organizationUnit,
                input.Sorting,
                input.MaxResultCount,
                input.SkipCount,
                input.Filter);

            return new PagedResultDto<IdentityRoleDto>(
              count,
              ObjectMapper.Map<List<IdentityRole>, List<IdentityRoleDto>>(identityRoles)
              );
        }

        /// <summary>
        /// Get members paged list
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(IdentityPermissions.OrganizationUnits.Default)]
        public virtual async Task<PagedResultDto<IdentityUserDto>> GetMembersPagedListAsync(Guid id, OrganizationUnitMemberPagedRequestDto input)
        {
            var organizationUnit = await OrganizationUnitRepository.GetAsync(id);

            var count = await OrganizationUnitRepository.GetMembersCountAsync(organizationUnit, input.Filter);
            var identityUsers = await OrganizationUnitRepository.GetMembersAsync(
                organizationUnit,
                input.Sorting,
                input.MaxResultCount,
                input.SkipCount,
                input.Filter);

            return new PagedResultDto<IdentityUserDto>(
              count,
              ObjectMapper.Map<List<IdentityUser>, List<IdentityUserDto>>(identityUsers)
              );
        }

        /// <summary>
        /// Get unAdded members paged list
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(IdentityPermissions.OrganizationUnits.Default)]
        public virtual async Task<PagedResultDto<IdentityUserDto>> GetUnaddedMembersPagedListAsync(Guid id, OrganizationUnitUnaddedMemberPagedRequestDto input)
        {
            var organizationUnit = await OrganizationUnitRepository.GetAsync(id);

            var count = await OrganizationUnitRepository.GetUnaddedUsersCountAsync(organizationUnit, input.Filter);
            var identityUsers = await OrganizationUnitRepository.GetUnaddedUsersAsync(
                organizationUnit,
                input.Sorting,
                input.MaxResultCount,
                input.SkipCount,
                input.Filter);

            return new PagedResultDto<IdentityUserDto>(
              count,
              ObjectMapper.Map<List<IdentityUser>, List<IdentityUserDto>>(identityUsers)
              );
        }

        /// <summary>
        /// Create
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(IdentityPermissions.OrganizationUnits.Create)]
        public virtual async Task<Guid> CreateAsync(CreateOrganizationUnitDto input)
        {
            var organizationUnit = new OrganizationUnit(
                GuidGenerator.Create(),
                input.DisplayName,
                input.ParentId,
                CurrentTenant.Id);

            await OrganizationUnitManager.CreateAsync(organizationUnit);
            return organizationUnit.Id;
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(IdentityPermissions.OrganizationUnits.Update)]
        public virtual async Task UpdateAsync(Guid id, UpdateOrganizationUnitDto input)
        {
            var organizationUnit = await OrganizationUnitRepository.GetAsync(id);
            organizationUnit.DisplayName = input.DisplayName;
            await OrganizationUnitManager.UpdateAsync(organizationUnit);
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(IdentityPermissions.OrganizationUnits.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await OrganizationUnitManager.DeleteAsync(id);
        }

        /// <summary>
        /// Move org to new parient
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(IdentityPermissions.OrganizationUnits.Update)]
        public virtual async Task MoveAsync(Guid id, MoveOrganizationUnitDto input)
        {
            await OrganizationUnitManager.MoveAsync(id, input.NewParentId);
        }

        /// <summary>
        /// Add role to organizationUnit
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(IdentityPermissions.OrganizationUnits.Update)]
        public virtual async Task AddRoleToOrganizationUnitAsync(Guid id, AddRoleToOrganizationUnitDto input)
        {
            foreach (var roleId in input.RoleIds)
            {
                await OrganizationUnitManager.AddRoleToOrganizationUnitAsync(roleId, id);
            }
        }

        /// <summary>
        /// Remove role from organizationUnit
        /// </summary>
        /// <param name="id"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [Authorize(IdentityPermissions.OrganizationUnits.Update)]
        public virtual async Task RemoveRoleFromOrganizationUnitAsync(Guid id, Guid roleId)
        {
            await OrganizationUnitManager.RemoveRoleFromOrganizationUnitAsync(roleId, id);
        }

        /// <summary>
        /// Add members to organizationUnit
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(IdentityPermissions.OrganizationUnits.Update)]
        public virtual async Task AddMemberToOrganizationUnitAsync(Guid id, AddMemberToOrganizationUnitDto input)
        {
            foreach (var userId in input.UserIds)
            {
                await IdentityUserManager.AddToOrganizationUnitAsync(userId, id);
            }
        }

        /// <summary>
        /// Remove member from organizationUnit
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Authorize(IdentityPermissions.OrganizationUnits.Update)]
        public virtual async Task RemoveMemberFromOrganizationUnitAsync(Guid id, Guid userId)
        {
            await IdentityUserManager.RemoveFromOrganizationUnitAsync(userId, id);
        }

    }
}
