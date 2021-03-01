namespace SharpAbp.Abp.FileStoring
{
    public interface IFileNormalizeNamingService
    {
        FileNormalizeNaming NormalizeNaming(FileContainerConfiguration configuration, string containerName, string fileName);

        string NormalizeContainerName(FileContainerConfiguration configuration, string containerName);

        string NormalizeFileName(FileContainerConfiguration configuration, string fileName);
    }
}
