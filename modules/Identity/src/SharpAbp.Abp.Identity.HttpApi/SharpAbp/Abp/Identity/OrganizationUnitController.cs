using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Identity;

namespace SharpAbp.Abp.Identity
{
    [RemoteService(Name = IdentityRemoteServiceConsts.RemoteServiceName)]
    [Area("identity")]
    [Route("api/identity/organization-units")]
    public class OrganizationUnitController : IdentityController, IOrganizationUnitAppService
    {
        private readonly IOrganizationUnitAppService _organizationUnitAppService;
        public OrganizationUnitController(IOrganizationUnitAppService organizationUnitAppService)
        {
            _organizationUnitAppService = organizationUnitAppService;
        }

        /// <summary>
        /// Get all organizationUnits
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("all")]
        public async Task<List<OrganizationUnitDto>> GetAllAsync()
        {
            return await _organizationUnitAppService.GetAllAsync();
        }

        /// <summary>
        /// Find organizationUnit by displayName
        /// </summary>
        /// <param name="displayName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("find-by-displayName/{displayName}")]
        public async Task<OrganizationUnitDto> FindByDisplayNameAsync(string displayName)
        {
            return await _organizationUnitAppService.FindByDisplayNameAsync(displayName);
        }

        /// <summary>
        /// Get by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<OrganizationUnitDto> GetAsync(Guid id)
        {
            return await _organizationUnitAppService.GetAsync(id);
        }

        /// <summary>
        /// Get list by ids
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-list")]
        public async Task<List<OrganizationUnitDto>> GetListAsync(List<Guid> ids)
        {
            return await _organizationUnitAppService.GetListAsync(ids);
        }

        /// <summary>
        /// Get children by parentId
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-children/{parentId}")]
        public async Task<List<OrganizationUnitDto>> GetChildrenAsync(Guid? parentId = null)
        {
            return await _organizationUnitAppService.GetChildrenAsync(parentId);
        }

        /// <summary>
        /// Get all children with parent code
        /// </summary>
        /// <param name="code"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-all-children/{code}")]
        public async Task<List<OrganizationUnitDto>> GetAllChildrenWithParentCodeAsync(string code, Guid? parentId = null)
        {
            return await _organizationUnitAppService.GetAllChildrenWithParentCodeAsync(code, parentId);
        }

        /// <summary>
        /// Paged list
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<PagedResultDto<OrganizationUnitDto>> GetPagedListAsync(OrganizationUnitPagedRequestDto input)
        {
            return await _organizationUnitAppService.GetPagedListAsync(input);
        }

        /// <summary>
        /// Create organizationUnit
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<Guid> CreateAsync(CreateOrganizationUnitDto input)
        {
            return await _organizationUnitAppService.CreateAsync(input);
        }

        /// <summary>
        /// Update organizationUnit
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}")]
        public async Task UpdateAsync(Guid id, UpdateOrganizationUnitDto input)
        {
            await _organizationUnitAppService.UpdateAsync(id, input);
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task DeleteAsync(Guid id)
        {
            await _organizationUnitAppService.DeleteAsync(id);
        }

        /// <summary>
        /// Move
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("move/{id}")]
        public async Task MoveAsync(Guid id, MoveOrganizationUnitDto input)
        {
            await _organizationUnitAppService.MoveAsync(id, input);
        }

        /// <summary>
        /// Get roles paged list
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}/roles")]
        public async Task<PagedResultDto<IdentityRoleDto>> GetRolesPagedListAsync(Guid id, OrganizationUnitRolePagedRequestDto input)
        {
            return await _organizationUnitAppService.GetRolesPagedListAsync(id, input);
        }

        /// <summary>
        /// Get unAdded roles
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}/roles/unAdded")]
        public async Task<PagedResultDto<IdentityRoleDto>> GetUnaddedRolesPagedListAsync(Guid id, OrganizationUnitUnaddedRolePagedRequestDto input)
        {
            return await _organizationUnitAppService.GetUnaddedRolesPagedListAsync(id, input);
        }

        /// <summary>
        /// Add role to organizationUnit
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{id}/roles")]
        public async Task AddRoleToOrganizationUnitAsync(Guid id, AddRoleToOrganizationUnitDto input)
        {
            await _organizationUnitAppService.AddRoleToOrganizationUnitAsync(id, input);
        }

        /// <summary>
        /// Remove role from organizationUnit
        /// </summary>
        /// <param name="id"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}/roles/{roleId}")]
        public async Task RemoveRoleFromOrganizationUnitAsync(Guid id, Guid roleId)
        {
            await _organizationUnitAppService.RemoveRoleFromOrganizationUnitAsync(id, roleId);
        }

        /// <summary>
        /// Get members paged list
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}/members")]
        public async Task<PagedResultDto<IdentityUserDto>> GetMembersPagedListAsync(Guid id, OrganizationUnitMemberPagedRequestDto input)
        {
            return await _organizationUnitAppService.GetMembersPagedListAsync(id, input);
        }

        /// <summary>
        /// Get unAdded members
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}/members/unAdded")]
        public async Task<PagedResultDto<IdentityUserDto>> GetUnaddedMembersPagedListAsync(Guid id, OrganizationUnitUnaddedMemberPagedRequestDto input)
        {
            return await _organizationUnitAppService.GetUnaddedMembersPagedListAsync(id, input);
        }

        [HttpPost]
        [Route("{id}/members")]
        public async Task AddMemberToOrganizationUnitAsync(Guid id, AddMemberToOrganizationUnitDto input)
        {
            await _organizationUnitAppService.AddMemberToOrganizationUnitAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}/members/{userId}")]
        public async Task RemoveMemberFromOrganizationUnitAsync(Guid id, Guid userId)
        {
            await _organizationUnitAppService.RemoveMemberFromOrganizationUnitAsync(id, userId);
        }
    }
}
