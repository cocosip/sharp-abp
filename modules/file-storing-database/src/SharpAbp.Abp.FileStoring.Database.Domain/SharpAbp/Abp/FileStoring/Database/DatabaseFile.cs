using JetBrains.Annotations;
using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace SharpAbp.Abp.FileStoring.Database
{
    public class DatabaseFile : AggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid ContainerId { get; protected set; }

        public virtual Guid? TenantId { get; protected set; }

        public virtual string Name { get; protected set; }

        public virtual byte[] Content { get; protected set; }

        public DatabaseFile(Guid id, Guid containerId, [NotNull] string name, [NotNull] byte[] content, Guid? tenantId = null)
            : base(id)
        {
            Name = Check.NotNullOrWhiteSpace(name, nameof(name), DatabaseFileConsts.MaxNameLength);
            ContainerId = containerId;
            Content = CheckContentLength(content);
            TenantId = tenantId;
        }

        public virtual void SetContent(byte[] content)
        {
            Content = CheckContentLength(content);
        }

        protected virtual byte[] CheckContentLength(byte[] content)
        {
            Check.NotNull(content, nameof(content));

            if (content.Length >= DatabaseFileConsts.MaxContentLength)
            {
                throw new AbpException($"File content size cannot be more than {DatabaseFileConsts.MaxContentLength} Bytes.");
            }

            return content;
        }
    }
}
