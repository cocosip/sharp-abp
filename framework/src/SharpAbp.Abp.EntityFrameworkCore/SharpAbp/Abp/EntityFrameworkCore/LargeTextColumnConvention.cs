using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharpAbp.Abp.Data;
using System.Collections.Generic;
using System.Linq;

namespace SharpAbp.Abp.EntityFrameworkCore
{
    /// <summary>
    /// Maps bounded string and byte[] columns that are longer than the database provider can store
    /// inline to its large object type. The convention runs when the model is finalized, so it sees
    /// what the modules configured and applies to every AbpDbContext without a per module change.
    /// Columns with an explicit column type (HasColumnType), keys, indexes, fixed length columns,
    /// concurrency tokens and columns with a value converter are never changed.
    /// </summary>
    public class LargeTextColumnConvention : IModelFinalizingConvention
    {
        protected virtual string? ProviderName { get; }

        protected virtual IOptions<AbpLargeTextColumnOptions> Options { get; }

        protected virtual IEfCoreDatabaseProviderAccessor DatabaseProviderAccessor { get; }

        protected virtual ILogger<LargeTextColumnConvention> Logger { get; }

        public LargeTextColumnConvention(
            string? providerName,
            IOptions<AbpLargeTextColumnOptions> options,
            IEfCoreDatabaseProviderAccessor databaseProviderAccessor,
            ILogger<LargeTextColumnConvention> logger)
        {
            ProviderName = providerName;
            Options = options;
            DatabaseProviderAccessor = databaseProviderAccessor;
            Logger = logger;
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

            foreach (var entityType in modelBuilder.Metadata.GetEntityTypes())
            {
                // Evaluated before the conversions so the row size of a table is not underestimated.
                var rowSize = providerOptions.MaxRowSizeBytes > 0 ? EstimateRowSize(entityType) : 0;

                foreach (var property in GetCandidates(entityType, options.IncludeBinary))
                {
                    var maxLength = property.GetMaxLength();
                    if (maxLength == null)
                    {
                        continue;
                    }

                    if (property.ClrType == typeof(string))
                    {
                        if (providerOptions.StringMaxLength is int stringMaxLength &&
                            maxLength > stringMaxLength &&
                            providerOptions.StringColumnType != null)
                        {
                            ApplyColumnType(property, providerOptions.StringColumnType);
                        }
                    }
                    else if (providerOptions.BinaryMaxLength is int binaryMaxLength &&
                             maxLength > binaryMaxLength &&
                             providerOptions.BinaryColumnType != null)
                    {
                        ApplyColumnType(property, providerOptions.BinaryColumnType);
                    }
                }

                if (providerOptions.MaxRowSizeBytes > 0 && rowSize > providerOptions.MaxRowSizeBytes)
                {
                    Logger.LogInformation(
                        "Large text column convention detected a possible row size of {RowSize} bytes for {EntityType}, the limit of {ProviderName} is {MaxRowSize} bytes.",
                        rowSize,
                        entityType.Name,
                        ProviderName,
                        providerOptions.MaxRowSizeBytes);

                    foreach (var property in GetCandidates(entityType, options.IncludeBinary))
                    {
                        if ((property.GetMaxLength() ?? 0) < providerOptions.RowSizeConversionMinLength)
                        {
                            continue;
                        }

                        var columnType = property.ClrType == typeof(string)
                            ? providerOptions.StringColumnType
                            : providerOptions.BinaryColumnType;

                        if (columnType != null)
                        {
                            ApplyColumnType(property, columnType);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns the bounded string and byte[] columns that take part in the conversion.
        /// </summary>
        protected virtual IEnumerable<IConventionProperty> GetCandidates(
            IConventionEntityType entityType,
            bool includeBinary)
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType != typeof(string) &&
                    !(includeBinary && property.ClrType == typeof(byte[])))
                {
                    continue;
                }

                // Only bounded columns, and never override an explicitly configured column type.
                if (property.GetMaxLength() == null ||
                    property.FindAnnotation(RelationalAnnotationNames.ColumnType) != null)
                {
                    continue;
                }

                if (property.IsFixedLength() == true ||
                    property.GetValueConverter() != null ||
                    property.IsConcurrencyToken)
                {
                    continue;
                }

                if (entityType.FindDiscriminatorProperty() == property)
                {
                    continue;
                }

                if (property.IsKey() ||
                    property.IsForeignKey() ||
                    entityType.GetIndexes().Any(index => index.Properties.Contains(property)))
                {
                    Logger.LogWarning(
                        "Large text column convention skipped {EntityType}.{PropertyName} because it is used by a key or an index, configure its column type explicitly if the configured length is not supported.",
                        entityType.Name,
                        property.Name);
                    continue;
                }

                yield return property;
            }
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


