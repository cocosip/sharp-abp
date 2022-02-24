using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    public class UpdateDatabaseConnectionInfoDto : ExtensibleEntityDto
    {
        ///// <summary>
        ///// DbConnection name
        ///// </summary>
        //[Required]
        //[DynamicStringLength(typeof(DatabaseConnectionInfoConsts), nameof(DatabaseConnectionInfoConsts.MaxNameLength))]
        //public string Name { get; set; }

        /// <summary>
        /// Database Provider
        /// </summary>
        [Required]
        [DynamicStringLength(typeof(DatabaseConnectionInfoConsts), nameof(DatabaseConnectionInfoConsts.MaxConnectionStringLength))]
        public string DatabaseProvider { get; set; }

        /// <summary>
        /// Database connection string
        /// </summary>
        [DynamicStringLength(typeof(DatabaseConnectionInfoConsts), nameof(DatabaseConnectionInfoConsts.MaxConnectionStringLength))]
        public string ConnectionString { get; set; }
    }
}
