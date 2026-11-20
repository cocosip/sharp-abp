using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharpAbp.Abp.Data;
using System.Linq;

namespace SharpAbp.Abp.EntityFrameworkCore
{
    /// <summary>
    /// Maps string and byte[] columns to the large object type of the database provider when the
    /// configured length is above what the provider stores inline, when the table would exceed its
    /// row size, or when the property is marked with <see cref="LargeTextColumnAttribute"/>. The
    /// convention runs when the model is finalized, so it sees what the modules configured and
    /// applies to every AbpDbContext without a per module change. Columns with an explicit column
    /// type (HasColumnType), keys, indexes, fixed length columns, concurrency tokens and columns with a
    /// value converter are never changed.
    /// </summary>
    public class LargeTextColumnConvention : IModelFinalizingConvention
    {
        protected virtual string? ProviderName { get; }

        protected virtual IOptions<AbpLargeTextColumnOptions> Options { get; }

        protected virtual IEfCoreDatabaseProviderAccessor DatabaseProviderAccessor { get; }

        protected virtual ILogger<LargeTextColumnConvention> Logger { get; }

        protected virtual OceanBaseDatabaseMode? OceanBaseMode { get; }

        public LargeTextColumnConvention(
            string? providerName,
            IOptions<AbpLargeTextColumnOptions> options,
            IEfCoreDatabaseProviderAccessor databaseProviderAccessor,
            ILogger<LargeTextColumnConvention> logger)
            : this(providerName, options, databaseProviderAccessor, logger, null)
        {
        }

        public LargeTextColumnConvention(
            string? providerName,
            IOptions<AbpLargeTextColumnOptions> options,
            IEfCoreDatabaseProviderAccessor databaseProviderAccessor,
            ILogger<LargeTextColumnConvention> logger,
            OceanBaseDatabaseMode? oceanBaseMode)
        {
            ProviderName = providerName;
            Options = options;
            DatabaseProviderAccessor = databaseProviderAccessor;
            Logger = logger;
            OceanBaseMode = oceanBaseMode;
        }

        public virtual void ProcessModelFinalizing(
            IConventionModelBuilder modelBuilder,
            IConventionContext<IConventionModelBuilder> context)
        {
            var options = Options.Value;
            if (!options.Enabled || string.IsNullOrWhiteSpace(ProviderName))
            {
                return;
            }

            var databaseProvider = DatabaseProviderAccessor.GetDatabaseProviderOrNullByEfCoreName(ProviderName);
            if (databaseProvider == null || !options.Providers.TryGetValue(databaseProvider.Value, out var providerOptions))
            {
                return;
            }

            if (databaseProvider == DatabaseProvider.OceanBase && OceanBaseMode == OceanBaseDatabaseMode.MySql)
            {
                providerOptions = options.OceanBaseMySql;
            }

            foreach (var entityType in modelBuilder.Metadata.GetEntityTypes())
            {
                // Evaluated before the conversions so the row size of a table is not underestimated.
                var rowSize = providerOptions.MaxRowSizeBytes > 0 ? EstimateRowSize(entityType) : 0;
                var rowSizeExceeded = providerOptions.MaxRowSizeBytes > 0 && rowSize > providerOptions.MaxRowSizeBytes;
                if (rowSizeExceeded)
                {
                    Logger.LogInformation(
                        "Large text column convention detected a possible row size of {RowSize} bytes for {EntityType}, the limit of {ProviderName} is {MaxRowSize} bytes.",
                        rowSize,
                        entityType.Name,
                        ProviderName,
                        providerOptions.MaxRowSizeBytes);
                }

                foreach (var property in entityType.GetProperties())
                {
                    if (!ShouldConsider(entityType, property, options.IncludeBinary))
                    {
                        continue;
                    }

                    var maxLength = property.GetMaxLength();
                    var needsConversion =
                        HasLargeTextColumnAttribute(property) ||
                        IsAboveInlineLimit(property, maxLength, providerOptions) ||
                        (rowSizeExceeded && (maxLength ?? 0) >= providerOptions.RowSizeConversionMinLength);

                    if (!needsConversion)
                    {
                        continue;
                    }

                    if (property.IsKey() ||
                        property.IsForeignKey() ||
                        IsIndexed(entityType, property))
                    {
                        Logger.LogWarning(
                            "Large text column convention skipped {EntityType}.{PropertyName} because it is used by a key or an index, configure its column type explicitly when the configured length is not supported.",
                            entityType.Name,
                            property.Name);
                        continue;
                    }

                    var columnType = ResolveColumnType(property, providerOptions);
                    if (columnType == null)
                    {
                        Logger.LogWarning(
                            "Large text column convention skipped {EntityType}.{PropertyName} because no large text column type is configured for {ProviderName}.",
                            entityType.Name,
                            property.Name,
                            ProviderName);
                        continue;
                    }

                    ApplyColumnType(property, columnType);
                }
            }
        }

        /// <summary>
        /// Checks whether the property can be mapped to a large object column at all.
        /// </summary>
        protected virtual bool ShouldConsider(
            IConventionEntityType entityType,
            IConventionProperty property,
            bool includeBinary)
        {
            if (property.ClrType != typeof(string) &&
                !(includeBinary && property.ClrType == typeof(byte[])))
            {
                return false;
            }

            // An explicitly configured column type always wins.
            if (property.FindAnnotation(RelationalAnnotationNames.ColumnType) != null)
            {
                return false;
            }

            if (property.IsFixedLength() == true ||
                property.GetValueConverter() != null ||
                property.IsConcurrencyToken)
            {
                return false;
            }

            return entityType.FindDiscriminatorProperty() != property;
        }

        protected virtual bool HasLargeTextColumnAttribute(IConventionProperty property)
        {
            return property.PropertyInfo?.IsDefined(typeof(LargeTextColumnAttribute), false) == true;
        }

        protected virtual bool IsAboveInlineLimit(
            IConventionProperty property,
            int? maxLength,
            AbpLargeTextColumnProviderOptions providerOptions)
        {
            if (maxLength == null)
            {
                return false;
            }

            return property.ClrType == typeof(string)
                ? providerOptions.StringMaxLength is int stringMaxLength && maxLength > stringMaxLength
                : providerOptions.BinaryMaxLength is int binaryMaxLength && maxLength > binaryMaxLength;
        }

        protected virtual bool IsIndexed(IConventionEntityType entityType, IConventionProperty property)
        {
            return entityType.GetIndexes().Any(index => index.Properties.Contains(property));
        }

        protected virtual string? ResolveColumnType(
            IConventionProperty property,
            AbpLargeTextColumnProviderOptions providerOptions)
        {
            if (property.ClrType != typeof(string))
            {
                return providerOptions.BinaryColumnType;
            }

            return property.IsUnicode() == false
                ? providerOptions.NonUnicodeStringColumnType ?? providerOptions.StringColumnType
                : providerOptions.StringColumnType;
        }

        /// <summary>
        /// Estimates the potential row size in bytes of the string and binary columns that still use
        /// a bounded column type.
        /// </summary>
        protected virtual long EstimateRowSize(IConventionEntityType entityType)
        {
            long rowSize = 0;
            foreach (var property in entityType.GetProperties())
            {
                if (property.FindAnnotation(RelationalAnnotationNames.ColumnType) != null)
                {
                    continue;
                }

                var maxLength = property.GetMaxLength();
                if (maxLength == null)
                {
                    continue;
                }

                if (property.ClrType == typeof(string))
                {
                    rowSize += (long)maxLength.Value * (property.IsUnicode() == false ? 1 : 4);
                }
                else if (property.ClrType == typeof(byte[]))
                {
                    rowSize += maxLength.Value;
                }
            }

            return rowSize;
        }

        protected virtual void ApplyColumnType(IConventionProperty property, string columnType)
        {
            if (property.FindAnnotation(RelationalAnnotationNames.ColumnType)?.Value is string currentColumnType &&
                currentColumnType == columnType)
            {
                return;
            }

            property.SetAnnotation(RelationalAnnotationNames.ColumnType, columnType, true);

            Logger.LogInformation(
                "Large text column convention mapped {EntityType}.{PropertyName} ({MaxLength}) to {ColumnType} for {ProviderName}.",
                property.DeclaringType.Name,
                property.Name,
                property.GetMaxLength(),
                columnType,
                ProviderName);
        }
    }
}
