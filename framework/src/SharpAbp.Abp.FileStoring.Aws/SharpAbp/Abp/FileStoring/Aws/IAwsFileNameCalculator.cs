namespace SharpAbp.Abp.FileStoring.Aws
{
    public interface IAwsFileNameCalculator
    {
        string Calculate(FileProviderArgs args);
    }
}
