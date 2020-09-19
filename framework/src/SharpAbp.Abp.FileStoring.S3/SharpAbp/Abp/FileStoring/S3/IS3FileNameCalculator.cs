namespace SharpAbp.Abp.FileStoring.S3
{
    public interface IS3FileNameCalculator
    {
        string Calculate(FileProviderArgs args);
    }
}
