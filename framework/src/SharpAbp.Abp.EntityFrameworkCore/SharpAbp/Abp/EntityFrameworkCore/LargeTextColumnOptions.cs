using SharpAbp.Abp.Data;
using System.Collections.Generic;

namespace SharpAbp.Abp.EntityFrameworkCore
{
    /// <summary>
    /// Options for <see cref="LargeTextColumnConvention"/>. Only the providers configured in
    /// <see cref="Providers"/> are converted, because the other providers already widen long columns
    /// by themselves:
    /// <list type="bullet">
    /// <item>Oracle maps strings above 2000 characters to NCLOB and binary above 2000 bytes to BLOB</item>
    /// <item>SQL Server maps strings above 4000 characters to nvarchar(max) or varchar(max)</item>
    /// <item>PostgreSQL stores character varying up to 10485760 characters and longer strings as text</item>
    /// <item>SQLite ignores the configured length and stores the column as TEXT</item>
    /// </list>
    /// </summary>
    public class AbpLargeTextColumnOptions
    {
        /// <summary>
        /// Enables the convention. Default: true.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Also converts byte[] columns. Default: true.
        /// </summary>
        public bool IncludeBinary { get; set; } = true;

        /// <summary>
        /// Settings per database provider. A provider missing from this dictionary is not converted.
        /// </summary>
        public Dictionary<DatabaseProvider, AbpLargeTextColumnProviderOptions> Providers { get; set; } = new()
        {
            [DatabaseProvider.MySql] = new()
            {
                // Pomelo keeps varchar(n) up to 65535 / character width and ignores the 65535 byte
                // row size limit, so long columns have to be mapped to a LOB explicitly.
                StringMaxLength = 16383,
                StringColumnType = "longtext",
                BinaryMaxLength = 65535,
                BinaryColumnType = "longblob",
                MaxRowSizeBytes = 65535,
                RowSizeConversionMinLength = 2000
            },
            [DatabaseProvider.Dm] = new()
            {
                // DM keeps NVARCHAR2(n), counts the length in bytes by default and limits the
                // create table length by the database page size, so long columns have to be a LOB.
                StringMaxLength = 2000,
                StringColumnType = "CLOB",
                BinaryMaxLength = 2000,
                BinaryColumnType = "BLOB"
            },
            [DatabaseProvider.OceanBase] = new()
            {
                // OceanBase Oracle-compatible mode limits NVARCHAR2 to 32767 and VARCHAR2 in SQL to
                // 4000 bytes, so columns above the NVARCHAR2 limit have to be a CLOB.
                StringMaxLength = 32767,
                StringColumnType = "CLOB",
                BinaryMaxLength = 2000,
                BinaryColumnType = "BLOB"
            }
        };
    }

    /// <summary>
    /// Large text column settings of one database provider.
    /// </summary>
    public class AbpLargeTextColumnProviderOptions
    {
        /// <summary>
        /// Maximum string length the provider can store inline. Null disables string conversion.
        /// </summary>
        public int? StringMaxLength { get; set; }

        /// <summary>
        /// Column type used for strings longer than <see cref="StringMaxLength"/>.
        /// </summary>
        public string? StringColumnType { get; set; }

        /// <summary>
        /// Maximum binary length the provider can store inline. Null disables binary conversion.
        /// </summary>
        public int? BinaryMaxLength { get; set; }

        /// <summary>
        /// Column type used for binary columns longer than <see cref="BinaryMaxLength"/>.
        /// </summary>
        public string? BinaryColumnType { get; set; }

        /// <summary>
        /// Maximum potential row size in bytes, including the string and binary columns that still
        /// use a bounded column type. 0 disables the table level check.
        /// </summary>
        public long MaxRowSizeBytes { get; set; }

        /// <summary>
        /// Minimum length of the columns that take part in the row size conversion.
        /// </summary>
        public int RowSizeConversionMinLength { get; set; }
    }
}
