namespace SharpAbp.Abp.FileStoring.FastDFS
{
    public interface IFastDFSFileNameCalculator
    {
        string Calculate(FileProviderArgs args);
    }
}
