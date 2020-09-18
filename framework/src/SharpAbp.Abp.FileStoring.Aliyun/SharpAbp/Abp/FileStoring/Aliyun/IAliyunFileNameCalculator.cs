namespace SharpAbp.Abp.FileStoring.Aliyun
{
    public interface IAliyunFileNameCalculator
    {
        string Calculate(FileProviderArgs args);
    }
}
