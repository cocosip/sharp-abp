namespace SharpAbp.Abp.CSRedisCore
{
    public class AbpCSRedisOptions
    {
        public CSRedisConfigurations Clients { get; }

        public AbpCSRedisOptions()
        {
            Clients = new CSRedisConfigurations();
        }
    }
}
