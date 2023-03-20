namespace SharpAbp.Abp.FileStoring
{
    public interface IFileNamingNormalizer
    {
        string NormalizeContainerName(string containerName);

        string NormalizeFileName(string fileName);
    }
}
