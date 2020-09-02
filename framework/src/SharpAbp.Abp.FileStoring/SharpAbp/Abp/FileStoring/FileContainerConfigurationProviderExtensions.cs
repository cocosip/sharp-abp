namespace Volo.Abp.FileStoring
{
    public static class FileContainerConfigurationProviderExtensions
    {
        public static FileContainerConfiguration Get<TContainer>(
            this IFileContainerConfigurationProvider configurationProvider)
        {
            return configurationProvider.Get(FileContainerNameAttribute.GetContainerName<TContainer>());
        }
    }
}