// This file is automatically generated by ABP framework to use MVC Controllers from CSharp
using SharpAbp.Abp.FileStoringManagement;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Http.Client.ClientProxying;
using Volo.Abp.Http.Modeling;

// ReSharper disable once CheckNamespace
namespace SharpAbp.Abp.FileStoringManagement;

[Dependency(ReplaceServices = true)]
[ExposeServices(typeof(IFileProviderAppService), typeof(FileProviderClientProxy))]
public partial class FileProviderClientProxy : ClientProxyBase<IFileProviderAppService>, IFileProviderAppService
{
    public virtual async Task<List<ProviderDto>> GetProvidersAsync()
    {
        return await RequestAsync<List<ProviderDto>>(nameof(GetProvidersAsync));
    }

    public virtual async Task<bool> HasProviderAsync(string provider)
    {
        return await RequestAsync<bool>(nameof(HasProviderAsync), new ClientProxyRequestTypeValue
        {
            { typeof(string), provider }
        });
    }

    public virtual async Task<ProviderOptionsDto> GetOptionsAsync(string provider)
    {
        return await RequestAsync<ProviderOptionsDto>(nameof(GetOptionsAsync), new ClientProxyRequestTypeValue
        {
            { typeof(string), provider }
        });
    }
}
