namespace SharpAbp.Abp.FileStoring.KS3
{
    public interface IKS3FileNameCalculator
    {
        string Calculate(FileProviderArgs args);
    }
}
