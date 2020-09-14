namespace SharpAbp.Abp.FileStoring
{
    public interface IFastDFSFileNameCalculator
    {
        string Calculate(FileProviderArgs args);
    }
}
