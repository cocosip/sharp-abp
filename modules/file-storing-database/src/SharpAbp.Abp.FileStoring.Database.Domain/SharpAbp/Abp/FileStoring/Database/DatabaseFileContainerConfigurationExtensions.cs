namespace SharpAbp.Abp.FileStoring.Database
{
    public static class DatabaseFileContainerConfigurationExtensions
    {
        public static FileContainerConfiguration UseDatabase(
         this FileContainerConfiguration containerConfiguration)
        {
            containerConfiguration.Provider = DatabaseFileProviderConsts.ProviderName;
            return containerConfiguration;
        }
    }
}
