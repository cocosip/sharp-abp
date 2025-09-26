using SharpAbp.Abp.Data;
using System;

namespace SharpAbp.Abp.DbConnections
{
    /// <summary>
    /// Represents database connection information including provider and connection string
    /// </summary>
    public class DbConnectionInfo : IEquatable<DbConnectionInfo>
    {
        /// <summary>
        /// Gets the database provider type
        /// </summary>
        public DatabaseProvider DatabaseProvider { get; }
        
        /// <summary>
        /// Gets or sets the connection string
        /// </summary>
        public string? ConnectionString { get; set; }

        /// <summary>
        /// Initializes a new instance of the DbConnectionInfo class
        /// </summary>
        public DbConnectionInfo()
        {

        }

        /// <summary>
        /// Initializes a new instance of the DbConnectionInfo class with specified provider and connection string
        /// </summary>
        /// <param name="databaseProvider">The database provider</param>
        /// <param name="connectionString">The connection string</param>
        public DbConnectionInfo(DatabaseProvider databaseProvider, string? connectionString)
        {
            DatabaseProvider = databaseProvider;
            ConnectionString = connectionString;
        }

        /// <summary>
        /// Determines whether the specified DbConnectionInfo is equal to the current DbConnectionInfo
        /// </summary>
        /// <param name="other">The DbConnectionInfo to compare with the current DbConnectionInfo</param>
        /// <returns>true if the specified DbConnectionInfo is equal to the current DbConnectionInfo; otherwise, false</returns>
        public bool Equals(DbConnectionInfo other)
        {
            if (other is null)
            {
                return false;
            }
            return DatabaseProvider == other.DatabaseProvider && ConnectionString == other.ConnectionString;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current DbConnectionInfo
        /// </summary>
        /// <param name="obj">The object to compare with the current DbConnectionInfo</param>
        /// <returns>true if the specified object is equal to the current DbConnectionInfo; otherwise, false</returns>
        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            return obj is DbConnectionInfo other && Equals(other);
        }

        /// <summary>
        /// Serves as the default hash function
        /// </summary>
        /// <returns>A hash code for the current DbConnectionInfo</returns>
        public override int GetHashCode()
        {
            return StringComparer.InvariantCulture.GetHashCode(ConnectionString)
                | DatabaseProvider.GetHashCode();
        }
    }
}