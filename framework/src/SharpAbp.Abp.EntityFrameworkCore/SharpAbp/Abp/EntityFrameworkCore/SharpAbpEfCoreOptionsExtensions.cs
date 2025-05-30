namespace SharpAbp.Abp.EntityFrameworkCore
{
    public static class SharpAbpEfCoreOptionsExtensions
    {

        public static string GetOracleSQLCompatibility(this SharpAbpEfCoreOptions options)
        {

            if (options.Properties.TryGetValue("OracleSQLCompatibility", out string? value))
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

                return value ?? "DatabaseVersion19";
            }

            return "DatabaseVersion19";
        }

        public static string GetOracleAllowedLogonVersionClient(this SharpAbpEfCoreOptions options)
        {

            if (options.Properties.TryGetValue("OracleAllowedLogonVersionClient", out string? value))
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

                return value ?? "DatabaseVersion19";
            }

            return "DatabaseVersion19";
        }

        public static string GetMySqlVersion(this SharpAbpEfCoreOptions options)
        {
            if (options.Properties.TryGetValue("MySqlVersion", out string? value))
            {
                return value ?? "5.6";
            }
            return "5.6";
        }

        public static string GetMySqlServerType(this SharpAbpEfCoreOptions options)
        {
            if (options.Properties.TryGetValue("MySqlServerType", out string? value))
            {
                return value ?? "MySql";
            }
            return "MySql";
        }

        public static string GetDefaultSchema(this SharpAbpEfCoreOptions options)
        {
            if (options.Properties.TryGetValue("DefaultSchema", out string? value))
            {
                return value ?? "";
            }
            return "";
        }
    }
}