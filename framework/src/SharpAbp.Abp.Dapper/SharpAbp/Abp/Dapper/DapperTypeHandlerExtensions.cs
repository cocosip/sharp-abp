using Dapper;

namespace SharpAbp.Abp.Dapper
{
    public static class DapperTypeHandlerExtensions
    {
        public static void ConfigureTypeHandlers()
        {
            SqlMapper.AddTypeHandler(new ExtraPropertyDictionaryTypeHandler());
        }
    }
}
