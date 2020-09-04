namespace SharpAbp.Abp.FileStoring
{
    public interface IS3FileNameCalculator
    {
        string Calculate(FileProviderArgs args);
    }
}
