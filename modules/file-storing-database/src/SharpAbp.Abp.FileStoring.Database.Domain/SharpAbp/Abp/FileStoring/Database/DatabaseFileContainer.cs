using JetBrains.Annotations;
using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace SharpAbp.Abp.FileStoring.Database
{
    public class DatabaseFileContainer : AggregateRoot<Guid>, IMultiTenant //TODO: Rename to BlobContainer
    {
        public virtual Guid? TenantId { get; protected set; }

        public virtual string Name { get; protected set; }

        public virtual bool HttpSupport { get; protected set; }

        public virtual bool IncludeContainer { get; protected set; }

        public virtual string HttpServer { get; protected set; }


        public DatabaseFileContainer(Guid id, [NotNull] string name, Guid? tenantId = null, bool httpSupport = false, bool includeContainer = false, string httpServer = "")
            : base(id)
        {
            Name = Check.NotNullOrWhiteSpace(name, nameof(name), DatabaseContainerConsts.MaxNameLength);
            TenantId = tenantId;

            HttpSupport = httpSupport;
            IncludeContainer = includeContainer;
            HttpServer = Check.Length(httpServer, nameof(httpServer), DatabaseContainerConsts.MaxHttpServerLength);
        }
    }
}
