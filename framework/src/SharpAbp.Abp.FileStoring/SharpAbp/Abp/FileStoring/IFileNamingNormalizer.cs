namespace Volo.Abp.FileStoring
{
    public interface IFileNamingNormalizer
    {
        string NormalizeContainerName(string containerName);

        string NormalizeBlobName(string blobName);
    }
}
