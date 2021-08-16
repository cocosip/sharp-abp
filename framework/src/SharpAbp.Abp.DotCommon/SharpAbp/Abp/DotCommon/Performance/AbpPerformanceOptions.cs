namespace SharpAbp.Abp.DotCommon.Performance
{
    public class AbpPerformanceOptions
    {
        public PerformanceConfigurations Configurations { get; }
        public AbpPerformanceOptions()
        {
            Configurations = new PerformanceConfigurations();
        }
    }
}
