using System;
using System.Linq;
using Dapper;

namespace SharpAbp.Abp.Dapper
{
    /// <summary>
    /// Registers configured mappings with Dapper's process-wide type handler registry.
    /// </summary>
    public static class DapperTypeHandlerRegistrar
    {
        public static void Register(SharpAbpDapperOptions options)
        {
            ArgumentNullException.ThrowIfNull(options);

            foreach (var mappingGroup in options.TypeMappings.GroupBy(mapping => NormalizeType(mapping.TargetType)))
            {
                var mappings = mappingGroup.ToArray();

#pragma warning disable CS0618
                var defaultDbType = SqlMapper.LookupDbType(mappingGroup.Key, "value", false, out _);
#pragma warning restore CS0618

                SqlMapper.RemoveTypeMap(mappingGroup.Key);

#pragma warning disable CS0618
                SqlMapper.LookupDbType(mappingGroup.Key, "value", false, out var fallbackHandler);
#pragma warning restore CS0618

                if (fallbackHandler is DispatchingTypeHandler existingDispatcher)
                {
                    defaultDbType = existingDispatcher.DefaultDbType;
                    fallbackHandler = existingDispatcher.FallbackHandler;
                }

                SqlMapper.AddTypeHandler(
                    mappingGroup.Key,
                    new DispatchingTypeHandler(mappingGroup.Key, mappings, defaultDbType, fallbackHandler));
            }
        }

        private static Type NormalizeType(Type type)
        {
            return Nullable.GetUnderlyingType(type) ?? type;
        }
    }
}
