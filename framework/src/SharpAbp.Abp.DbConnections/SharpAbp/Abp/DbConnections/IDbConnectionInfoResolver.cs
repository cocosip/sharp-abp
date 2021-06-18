using JetBrains.Annotations;
using System.Threading.Tasks;

namespace SharpAbp.Abp.DbConnections
{
    public interface IDbConnectionInfoResolver
    {
        /// <summary>
        /// Resolve DbConnectionInfo
        /// </summary>
        /// <param name="dbConnectionName"></param>
        /// <returns></returns>
        Task<DbConnectionInfo> ResolveAsync([NotNull] string dbConnectionName);
    }
}
