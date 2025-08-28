using Volo.Abp.Collections;

namespace SharpAbp.Abp.Data.SqlBuilder
{
    public class DataSqlBuilderOptions
    {
        public ITypeList<ISqlParamConversionContributor> SqlParamConversionContributors { get; set; }

        public DataSqlBuilderOptions()
        {
            SqlParamConversionContributors = new TypeList<ISqlParamConversionContributor>();
        }

    }
}