namespace Volo.Abp.FileStoring
{
    public class AbpFileStoringOptions
    {
        public FileContainerConfigurations Containers { get; }

        public AbpFileStoringOptions()
        {
            Containers = new FileContainerConfigurations();
        }
    }
}