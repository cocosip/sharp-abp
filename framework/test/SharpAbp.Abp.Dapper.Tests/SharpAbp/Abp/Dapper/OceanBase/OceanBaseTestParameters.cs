using System;
using System.Reflection;
using OceanBaseConnector;

namespace SharpAbp.Abp.Dapper
{
    /// <summary>
    /// Creates an OceanBase parameter stamped with a compatibility mode. A command sets that property through
    /// an internal setter when it creates the parameter, so tests have to write it directly.
    /// </summary>
    internal static class OceanBaseTestParameters
    {
        public static OceanBaseParameter Create(OceanBaseCompatibilityMode compatibilityMode)
        {
            var parameter = new OceanBaseParameter();
            typeof(OceanBaseParameter)
                .GetProperty(nameof(OceanBaseParameter.CompatibilityMode))!
                .GetSetMethod(nonPublic: true)!
                .Invoke(parameter, new object?[] { compatibilityMode });
            return parameter;
        }
    }
}
