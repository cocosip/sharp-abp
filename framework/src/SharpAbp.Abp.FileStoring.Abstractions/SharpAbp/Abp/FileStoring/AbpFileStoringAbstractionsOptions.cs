namespace SharpAbp.Abp.FileStoring
{
    public class AbpFileStoringAbstractionsOptions
    {
        public FileProviderConfigurations Providers { get; }
        public AbpFileStoringAbstractionsOptions()
        {
            Providers = new FileProviderConfigurations();
        }
    }
}
