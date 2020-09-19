namespace SharpAbp.Abp.FileStoring.Database
{
    public static class DatabaseFileContainerConfigurationExtensions
    {
        public static FileContainerConfiguration UseDatabase(
         this FileContainerConfiguration containerConfiguration)
        {
            containerConfiguration.ProviderType = typeof(DatabaseFileProvider);
            return containerConfiguration;
        }
    }
}
