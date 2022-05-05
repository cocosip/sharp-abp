namespace SharpAbp.Abp.FileStoring.Obs
{
    public interface IObsFileNameCalculator
    {
        string Calculate(FileProviderArgs args);
    }
}
