using System.Collections.Generic;

namespace SharpAbp.Abp.Dapper
{
    /// <summary>
    /// Configuration for provider-aware Dapper type mappings.
    /// </summary>
    public class SharpAbpDapperOptions
    {
        public IList<IDapperTypeMapping> TypeMappings { get; }

        public SharpAbpDapperOptions()
        {
            TypeMappings = new List<IDapperTypeMapping>();
        }
    }
}
