using MongoDB.Driver;
using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace SharpAbp.Abp.TransformSecurityManagement.MongoDB
{
    [ConnectionStringName(AbpTransformSecurityManagementDbProperties.ConnectionStringName)]
    public interface IAbpTransformSecurityManagementMongoDbContext : IAbpMongoDbContext
    {
        IMongoCollection<SecurityCredentialInfo> SecurityCredentialInfos { get; }

    }
}
