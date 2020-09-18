using Aliyun.OSS;

namespace SharpAbp.Abp.FileStoring.Aliyun
{
    public interface IOssClientFactory
    {
        IOss Create(AliyunFileProviderConfiguration args);
    }
}
