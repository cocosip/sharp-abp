namespace SharpAbp.Abp.FileStoring.FileSystem
{
    public interface IFilePathCalculator
    {
        string CalculateRelativePath(FileProviderArgs args);

        string Calculate(FileProviderArgs args);
    }
}
