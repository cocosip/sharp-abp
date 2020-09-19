namespace SharpAbp.Abp.FileStoring
{
    public class AbpFileStoringOptions
    {
        public FileContainerConfigurations Containers { get; }

        public FileProviderConfigurations Providers { get; }

        public AbpFileStoringOptions()
        {
            Containers = new FileContainerConfigurations();

            Providers = new FileProviderConfigurations();
        }
    }
}