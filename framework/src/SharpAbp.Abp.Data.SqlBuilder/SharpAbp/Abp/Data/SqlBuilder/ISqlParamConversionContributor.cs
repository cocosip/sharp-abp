using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.Abp.Data.SqlBuilder
{
    /// <summary>
    /// Defines a contributor for SQL parameter conversion that can process specific types of parameters
    /// based on database provider and entity type.
    /// </summary>
    public interface ISqlParamConversionContributor
    {
        /// <summary>
        /// Determines whether this contributor can handle the parameter conversion for the given context.
        /// </summary>
        /// <param name="context">The parameter binding context containing database provider, entity type, and input data</param>
        /// <param name="cancellationToken">A token to cancel the operation</param>
        /// <returns>True if this contributor can handle the conversion; otherwise, false</returns>
        Task<bool> IsMatchAsync(ParameterBindingContext context, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Processes the parameter conversion and returns the converted parameter object.
        /// </summary>
        /// <param name="context">The parameter binding context containing database provider, entity type, and input data</param>
        /// <param name="cancellationToken">A token to cancel the operation</param>
        /// <returns>The converted parameter object that can be used with the target database provider</returns>
        Task<object> ProcessAsync(ParameterBindingContext context, CancellationToken cancellationToken = default);
    }
}