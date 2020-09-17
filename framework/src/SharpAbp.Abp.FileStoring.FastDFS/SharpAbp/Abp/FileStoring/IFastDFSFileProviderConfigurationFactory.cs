namespace SharpAbp.Abp.FileStoring
{
    public interface IFastDFSFileProviderConfigurationFactory
    {
        bool AddIfNotContains(FastDFSFileProviderConfiguration configuration);

        FastDFSFileProviderConfiguration GetConfiguration(string name);
    }
}
