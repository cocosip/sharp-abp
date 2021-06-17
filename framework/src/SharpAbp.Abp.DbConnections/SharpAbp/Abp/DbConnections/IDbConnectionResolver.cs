using JetBrains.Annotations;
using System.Threading.Tasks;

namespace SharpAbp.Abp.DbConnections
{
    public interface IDbConnectionResolver
    {

        /// <summary>
        /// Resolve by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [NotNull]
        Task<DbConnectionInfo> ResolveAsync([NotNull] string name);
    }
}
