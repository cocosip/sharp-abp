namespace SharpAbp.Abp.FileStoring
{
    public interface IFilePathCalculator
    {
        string Calculate(FileProviderArgs args);
    }
}
