using System;
using Aliyun.OSS;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;

namespace SharpAbp.Abp.FileStoring.Aliyun
{
    public class AliyunOssClientPolicy : IPooledObjectPolicy<IOss>
    {
        protected IServiceProvider ServiceProvider { get; }
        protected AliyunFileProviderConfiguration AliyunFileProviderConfiguration { get; }
        public AliyunOssClientPolicy(
            IServiceProvider serviceProvider,
            AliyunFileProviderConfiguration aliyunFileProviderConfiguration)
        {
            ServiceProvider = serviceProvider;
            AliyunFileProviderConfiguration = aliyunFileProviderConfiguration;
        }

        public IOss Create()
        {
            // 创建临时作用域解析 Scoped 服务
            using var scope = ServiceProvider.CreateScope();
            var ossClientFactory = scope.ServiceProvider.GetRequiredService<IOssClientFactory>();
            return ossClientFactory.Create(AliyunFileProviderConfiguration);
        }

        public bool Return(IOss obj)
        {
            return obj != null;
        }
    }
}
