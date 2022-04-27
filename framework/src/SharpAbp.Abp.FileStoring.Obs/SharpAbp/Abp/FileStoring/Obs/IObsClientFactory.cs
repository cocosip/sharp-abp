using OBS;

namespace SharpAbp.Abp.FileStoring.Obs
{
    public interface IObsClientFactory
    {
        ObsClient Create(ObsFileProviderConfiguration configuration);
    }
}
