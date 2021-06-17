using System;

namespace SharpAbp.Abp.DbConnections
{
    [Serializable]
    public class DbConnectionInfo : IEquatable<DbConnectionInfo>
    {
        public DatabaseProvider DatabaseProvider { get; set; }
        public string ConnectionString { get; set; }

        public DbConnectionInfo()
        {

        }

        public DbConnectionInfo(DatabaseProvider databaseProvider, string connectionString)
        {
            DatabaseProvider = databaseProvider;
            ConnectionString = connectionString;
        }

        public bool Equals(DbConnectionInfo other)
        {
            if (other is null)
            {
                return false;
            }
            return DatabaseProvider == other.DatabaseProvider && ConnectionString == other.ConnectionString;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            return obj is DbConnectionInfo other && Equals(other);
        }

        public override int GetHashCode()
        {
            return StringComparer.InvariantCulture.GetHashCode(ConnectionString)
                | DatabaseProvider.GetHashCode();
        }

        public static bool operator ==(DbConnectionInfo c1, DbConnectionInfo c2)
        {
            if (c1 is null && c2 is null)
            {
                return true;
            }
            return c1?.GetHashCode() == c2?.GetHashCode();
        }

        public static bool operator !=(DbConnectionInfo c1, DbConnectionInfo c2)
        {
            if (c1 is null && c2 is null)
            {
                return false;
            }
            return c1?.GetHashCode() != c2?.GetHashCode();
        }
    }
}
