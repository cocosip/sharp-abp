namespace SharpAbp.Abp.FileStoring.FastDFS
{
    public interface IFastDFSFileProviderConfigurationFactory
    {
        bool AddIfNotContains(FastDFSFileProviderConfiguration configuration);

        FastDFSFileProviderConfiguration GetConfiguration(string name);
    }
}
