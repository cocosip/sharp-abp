using KS3;

namespace SharpAbp.Abp.FileStoring.KS3
{
    public interface IKS3ClientFactory
    {
        IKS3 Create(KS3FileProviderConfiguration args);
    }
}
