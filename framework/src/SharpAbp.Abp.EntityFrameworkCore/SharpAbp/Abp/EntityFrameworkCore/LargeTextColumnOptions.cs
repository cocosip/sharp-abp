using SharpAbp.Abp.Data;
using System.Collections.Generic;

namespace SharpAbp.Abp.EntityFrameworkCore
{
    /// <summary>
    /// Options of <see cref="LargeTextColumnConvention"/>. A property is converted when it is marked
    /// with <see cref="LargeTextColumnAttribute"/>, when its configured length is above the inline
    /// limit of the provider, or when the potential row size of the table is above the provider limit.
    /// The other providers already widen long columns by themselves (Oracle maps strings above 2000
    /// characters to NCLOB, SQL Server above 4000 to nvarchar(max) or varchar(max), PostgreSQL stores
    /// character varying up to 10485760 characters and longer strings as text, SQLite ignores the
    /// configured length and stores TEXT), so their entries only define the column type used for the
    /// marked properties.
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
        /// An entry replaces the built in one as a whole, so write all fields of that provider.
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
            },
            // The providers below widen long columns by themselves, the entries only define which
            // column type a property marked with LargeTextColumnAttribute is mapped to.
            [DatabaseProvider.Oracle] = new()
            {
                StringColumnType = "NCLOB",
                NonUnicodeStringColumnType = "CLOB",
                BinaryColumnType = "BLOB"
            },
            [DatabaseProvider.SqlServer] = new()
            {
                StringColumnType = "nvarchar(max)",
                NonUnicodeStringColumnType = "varchar(max)",
                BinaryColumnType = "varbinary(max)"
            },
            [DatabaseProvider.PostgreSql] = new()
            {
                StringColumnType = "text",
                BinaryColumnType = "bytea"
            },
            [DatabaseProvider.GaussDB] = new()
            {
                StringColumnType = "text",
                BinaryColumnType = "bytea"
            },
            [DatabaseProvider.OpenGauss] = new()
            {
                StringColumnType = "text",
                BinaryColumnType = "bytea"
            },
            [DatabaseProvider.Sqlite] = new()
            {
                StringColumnType = "TEXT",
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
        /// Maximum string length the provider can store inline. Null disables the length based
        /// conversion of strings.
        /// </summary>
        public int? StringMaxLength { get; set; }

        /// <summary>
        /// Column type used for strings longer than <see cref="StringMaxLength"/> and for string
        /// properties marked with <see cref="LargeTextColumnAttribute"/>.
        /// </summary>
        public string? StringColumnType { get; set; }

        /// <summary>
        /// Column type used for non Unicode strings. Null means <see cref="StringColumnType"/>.
        /// </summary>
        public string? NonUnicodeStringColumnType { get; set; }

        /// <summary>
        /// Maximum binary length the provider can store inline. Null disables the length based
        /// conversion of binary columns.
        /// </summary>
        public int? BinaryMaxLength { get; set; }

        /// <summary>
        /// Column type used for binary columns longer than <see cref="BinaryMaxLength"/> and for
        /// byte[] properties marked with <see cref="LargeTextColumnAttribute"/>.
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
