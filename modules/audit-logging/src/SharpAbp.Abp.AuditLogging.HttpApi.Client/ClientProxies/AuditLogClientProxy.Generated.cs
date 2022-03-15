// This file is automatically generated by ABP framework to use MVC Controllers from CSharp
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Http.Client;
using Volo.Abp.Http.Modeling;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Http.Client.ClientProxying;
using SharpAbp.Abp.AuditLogging;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace SharpAbp.Abp.AuditLogging.ClientProxies;

[Dependency(ReplaceServices = true)]
[ExposeServices(typeof(IAuditLogAppService), typeof(AuditLogClientProxy))]
public partial class AuditLogClientProxy : ClientProxyBase<IAuditLogAppService>, IAuditLogAppService
{
    public virtual async Task<AuditLogDto> GetAsync(Guid id)
    {
        return await RequestAsync<AuditLogDto>(nameof(GetAsync), new ClientProxyRequestTypeValue
        {
            { typeof(Guid), id }
        });
    }

    public virtual async Task<PagedResultDto<AuditLogDto>> GetPagedListAsync(AuditLogPagedRequestDto input)
    {
        return await RequestAsync<PagedResultDto<AuditLogDto>>(nameof(GetPagedListAsync), new ClientProxyRequestTypeValue
        {
            { typeof(AuditLogPagedRequestDto), input }
        });
    }

    public virtual async Task<PagedResultDto<EntityChangeDto>> GetEntityChangePagedListAsync(EntityChangePagedRequestDto input)
    {
        return await RequestAsync<PagedResultDto<EntityChangeDto>>(nameof(GetEntityChangePagedListAsync), new ClientProxyRequestTypeValue
        {
            { typeof(EntityChangePagedRequestDto), input }
        });
    }

    public virtual async Task<EntityChangeWithUsernameDto> GetEntityChangeWithUsernameAsync(Guid entityChangeId)
    {
        return await RequestAsync<EntityChangeWithUsernameDto>(nameof(GetEntityChangeWithUsernameAsync), new ClientProxyRequestTypeValue
        {
            { typeof(Guid), entityChangeId }
        });
    }

    public virtual async Task<List<EntityChangeWithUsernameDto>> GetEntityChangesWithUsernameAsync(string entityId, string entityTypeFullName)
    {
        return await RequestAsync<List<EntityChangeWithUsernameDto>>(nameof(GetEntityChangesWithUsernameAsync), new ClientProxyRequestTypeValue
        {
            { typeof(string), entityId },
            { typeof(string), entityTypeFullName }
        });
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        await RequestAsync(nameof(DeleteAsync), new ClientProxyRequestTypeValue
        {
            { typeof(Guid), id }
        });
    }
}