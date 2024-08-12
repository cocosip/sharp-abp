using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Volo.Abp.EntityFrameworkCore;

namespace SharpAbp.Abp.EntityFrameworkCore
{
    public static class ConfigurationExtensions
    {
        public static EfCoreDatabaseProvider GetDatabaseProvider(
            this IConfiguration configuration,
            EfCoreDatabaseProvider defaultValue = EfCoreDatabaseProvider.PostgreSql)
        {
            if (Enum.TryParse(configuration["EfCoreOptions:DatabaseProvider"], out EfCoreDatabaseProvider databaseProvider))
            {
                return databaseProvider;
            }

            return defaultValue;
        }

        public static Dictionary<string, string> GetProperties(this IConfiguration configuration)
        {
            var options = configuration.GetSection("EfCoreOptions").Get<SharpAbpEfCoreOptions>();
            return options.Properties;
        }

        public static string GetOracleSQLCompatibility(this IConfiguration configuration)
        {
            var properties = configuration.GetProperties();
            if (properties.TryGetValue("OracleSQLCompatibility", out string value))
            {
                switch (value)
                {
                    case "DatabaseVersion19":
                    case "DatabaseVersion21":
                    case "DatabaseVersion23":
                        return value;
                    default:
                        break;
                }

                return value;
            }
            return "DatabaseVersion19";
        }

        public static string GetOracleAllowedLogonVersionClient(this IConfiguration configuration)
        {
            var properties = configuration.GetProperties();
            if (properties.TryGetValue("OracleAllowedLogonVersionClient", out string value))
            {
                switch (value)
                {
                    case "Version8":
                    case "Version9":
                    case "Version10":
                    case "Version11":
                    case "Version12":
                    case "Version12a":
                        return value;
                    default:
                        break;
                }

                return value;
            }
            return "DatabaseVersion19";
        }

        public static string GetMySqlVersion(this IConfiguration configuration)
        {
            var properties = configuration.GetProperties();
            if (properties.TryGetValue("MySqlVersion", out string value))
            {
                return value;
            }
            return "5.6";
        }

        public static string GetMySqlServerType(this IConfiguration configuration)
        {
            var properties = configuration.GetProperties();
            if (properties.TryGetValue("MySqlServerType", out string value))
            {
                return value;
            }
            return "MySql";
        }
    }
}
