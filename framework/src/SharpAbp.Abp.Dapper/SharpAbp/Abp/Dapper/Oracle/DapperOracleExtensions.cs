using Dapper;
using System;

namespace SharpAbp.Abp.Dapper.Oracle
{
    /// <summary>
    /// Oracle-specific Dapper extensions for type handling
    /// </summary>
    public static class DapperOracleExtensions
    {
        /// <summary>
        /// Configures Oracle-specific type handlers for Dapper
        /// </summary>
        public static void ConfigureOracleTypeHandlers()
        {
            // Remove existing type maps to avoid conflicts
            SqlMapper.ResetTypeHandlers();
            SqlMapper.RemoveTypeMap(typeof(Guid));
            SqlMapper.RemoveTypeMap(typeof(Guid?));
            SqlMapper.RemoveTypeMap(typeof(bool));
            SqlMapper.RemoveTypeMap(typeof(bool?));

            // Add Oracle-specific type handlers
            SqlMapper.AddTypeHandler(new GuidTypeHandler());
            SqlMapper.AddTypeHandler(new NullableGuidTypeHandler());
            SqlMapper.AddTypeHandler(new BoolTypeHandler());
            SqlMapper.AddTypeHandler(new NullableBoolTypeHandler());
        }
    }
}