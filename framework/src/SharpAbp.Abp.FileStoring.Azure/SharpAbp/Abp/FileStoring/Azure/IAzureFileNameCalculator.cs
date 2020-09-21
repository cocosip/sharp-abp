namespace SharpAbp.Abp.FileStoring.Azure
{
    public interface IAzureFileNameCalculator
    {
        string Calculate(FileProviderArgs args);
    }
}
