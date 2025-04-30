using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace SharpAbp.Abp.FileStoringManagement
{
    public interface IContainerManager : IDomainService
    {
        Task<FileStoringContainer> CreateAsync(Guid? tenantId, bool isMultiTenant, string provider, string name, string title, bool enableAutoMultiPartUpload, int multiPartUploadMinFileSize, int multiPartUploadShardingSize, bool httpAccess, List<NameValue> values, CancellationToken cancellationToken = default);

        Task<FileStoringContainer> UpdateAsync(FileStoringContainer container, bool isMultiTenant, string provider, string name, string title, bool enableAutoMultiPartUpload, int multiPartUploadMinFileSize, int multiPartUploadShardingSize, bool httpAccess, List<NameValue> values, CancellationToken cancellationToken = default);

        void ValidateProviderValues(string provider, List<NameValue> values);

        Task ValidateNameAsync(Guid? tenantId, string name, Guid? expectedId = null, CancellationToken cancellationToken = default);
    }
}
