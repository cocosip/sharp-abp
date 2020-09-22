namespace SharpAbp.Abp.Consul
{
    public class AbpConsulOptions
    {
        public ConsulConfigurations Consuls { get; }

        public AbpConsulOptions()
        {
            Consuls = new ConsulConfigurations();
        }
    }
}
