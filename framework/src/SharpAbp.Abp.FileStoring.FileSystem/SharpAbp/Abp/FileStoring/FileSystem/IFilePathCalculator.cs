namespace SharpAbp.Abp.FileStoring.FileSystem
{
    public interface IFilePathCalculator
    {
        string Calculate(FileProviderArgs args);
    }
}
