namespace SharpAbp.Abp.FileStoring
{
    public class FileNormalizeNaming
    {
        public string? ContainerName { get; }
        public string? FileName { get; }

        public FileNormalizeNaming(string? containerName, string? fileName)
        {
            ContainerName = containerName;
            FileName = fileName;
        }
    }
}
