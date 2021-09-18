using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Identity;

namespace SharpAbp.Abp.Identity
{
    public interface IOrganizationUnitAppService : IApplicationService
    {
        /// <summary>
        /// Get by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<OrganizationUnitDto> GetAsync(Guid id);

        /// <summary>
        /// Get all
        /// </summary>
        /// <returns></returns>
        Task<List<OrganizationUnitDto>> GetAllAsync();

        /// <summary>
        /// Get paged list
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagedResultDto<OrganizationUnitDto>> GetPagedListAsync(OrganizationUnitPagedRequestDto input);

        /// <summary>
        /// Find by displayName
        /// </summary>
        /// <param name="displayName"></param>
        /// <returns></returns>
        Task<OrganizationUnitDto> FindByDisplayNameAsync(string displayName);

        /// <summary>
        /// Get children by pateintId
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        Task<List<OrganizationUnitDto>> GetChildrenAsync(Guid? parentId = null);

        /// <summary>
        /// GetAllChildrenWithParentCodeAsync
        /// </summary>
        /// <param name="code"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        Task<List<OrganizationUnitDto>> GetAllChildrenWithParentCodeAsync(string code, Guid? parentId = null);

        /// <summary>
        /// Get list by ids
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<List<OrganizationUnitDto>> GetListAsync(List<Guid> ids);

        /// <summary>
        /// Get roles paged list
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagedResultDto<IdentityRoleDto>> GetRolesPagedListAsync(Guid id, OrganizationUnitRolePagedRequestDto input);

        /// <summary>
        /// Get unAdded roles paged list
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagedResultDto<IdentityRoleDto>> GetUnaddedRolesPagedListAsync(Guid id, OrganizationUnitUnaddedRolePagedRequestDto input);

        /// <summary>
        /// Get members paged list
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagedResultDto<IdentityUserDto>> GetMembersPagedListAsync(Guid id, OrganizationUnitMemberPagedRequestDto input);

        /// <summary>
        /// Get unAdded members paged list
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagedResultDto<IdentityUserDto>> GetUnaddedMembersPagedListAsync(Guid id, OrganizationUnitUnaddedMemberPagedRequestDto input);

        /// <summary>
        /// Create
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Guid> CreateAsync(CreateOrganizationUnitDto input);

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        Task UpdateAsync(Guid id, UpdateOrganizationUnitDto input);

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Move org to new parient
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        Task MoveAsync(Guid id, MoveOrganizationUnitDto input);

        /// <summary>
        /// Add role to organizationUnit
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        Task AddRoleToOrganizationUnitAsync(Guid id, AddRoleToOrganizationUnitDto input);

        /// <summary>
        /// Remove role from organizationUnit
        /// </summary>
        /// <param name="id"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        Task RemoveRoleFromOrganizationUnitAsync(Guid id, Guid roleId);

        /// <summary>
        /// Add members to organizationUnit
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        Task AddMemberToOrganizationUnitAsync(Guid id, AddMemberToOrganizationUnitDto input);

        /// <summary>
        /// Remove member from organizationUnit
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task RemoveMemberFromOrganizationUnitAsync(Guid id, Guid userId);

    }

}
