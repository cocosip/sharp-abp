using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace SharpAbp.Abp.FileStoring
{
    /// <summary>
    /// Default implementation of <see cref="IFilePathBuilder"/>.
    /// Reads global options from <see cref="AbpFileStoringAbstractionsOptions.FilePathBuilder"/>
    /// and per-operation context from <see cref="IFilePathContextAccessor"/>.
    /// </summary>
    public class DefaultFilePathBuilder : IFilePathBuilder, ITransientDependency
    {
        protected ICurrentTenant CurrentTenant { get; }
        protected IFilePathContextAccessor FilePathContextAccessor { get; }
        protected AbpFileStoringAbstractionsOptions Options { get; }

        public DefaultFilePathBuilder(
            ICurrentTenant currentTenant,
            IFilePathContextAccessor filePathContextAccessor,
            IOptions<AbpFileStoringAbstractionsOptions> options)
        {
            CurrentTenant = currentTenant;
            FilePathContextAccessor = filePathContextAccessor;
            Options = options.Value;
        }

        public virtual string Build(FileProviderArgs args)
        {
            var builderOptions = Options.FilePathBuilder;
            var context = FilePathContextAccessor.Current;

            if (Options.FilePathStrategy == FilePathGenerationStrategy.DirectFileId)
            {
                var prefix = ResolvePrefix(builderOptions, context);
                return prefix.IsNullOrWhiteSpace()
                    ? args.FileId
                    : $"{prefix}/{args.FileId}";
            }

            // TenantBased
            var parts = new List<string>();

            var prefixValue = ResolvePrefix(builderOptions, context);
            if (!prefixValue.IsNullOrWhiteSpace())
            {
                parts.Add(prefixValue!);
            }

            if (CurrentTenant.Id == null)
            {
                if (!string.IsNullOrEmpty(builderOptions.HostSegment))
                {
                    parts.Add(builderOptions.HostSegment);
                }
            }
            else
            {
                var identifier = builderOptions.TenantIdentifierFactory != null
                    ? builderOptions.TenantIdentifierFactory(CurrentTenant.Id.Value, CurrentTenant.Name, context)
                    : CurrentTenant.Id.Value.ToString("D");

                parts.Add(string.IsNullOrEmpty(builderOptions.TenantsSegment)
                    ? identifier
                    : $"{builderOptions.TenantsSegment}/{identifier}");
            }

            parts.Add(args.FileId);
            return string.Join("/", parts);
        }

        /// <summary>
        /// Resolves the effective prefix.
        /// Priority: PrefixFactory > FilePathContext.Prefix > FilePathBuilderOptions.Prefix
        /// </summary>
        protected virtual string? ResolvePrefix(FilePathBuilderOptions options, FilePathContext? context)
        {
            if (options.PrefixFactory != null)
            {
                return options.PrefixFactory(context);
            }

            if (!string.IsNullOrEmpty(context?.Prefix))
            {
                return context!.Prefix;
            }

            return options.Prefix;
        }
    }
}
