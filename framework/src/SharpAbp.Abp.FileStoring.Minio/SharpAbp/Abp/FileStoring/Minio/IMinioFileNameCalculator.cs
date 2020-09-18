namespace SharpAbp.Abp.FileStoring.Minio
{
    public interface IMinioFileNameCalculator
    {
        string Calculate(FileProviderArgs args);
    }
}
