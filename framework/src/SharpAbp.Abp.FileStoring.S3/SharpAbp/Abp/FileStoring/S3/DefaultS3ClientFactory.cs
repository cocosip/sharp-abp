﻿using Amazon.S3;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.FileStoring.S3
{
    public class DefaultS3ClientFactory : IS3ClientFactory, ITransientDependency
    {

        public virtual IAmazonS3 Create(S3FileProviderConfiguration configuration)
        {
            Check.NotNullOrWhiteSpace(configuration.AccessKeyId, nameof(configuration.AccessKeyId));
            Check.NotNullOrWhiteSpace(configuration.SecretAccessKey, nameof(configuration.SecretAccessKey));
            Check.NotNullOrWhiteSpace(configuration.ServerUrl, nameof(configuration.ServerUrl));
            var clientConfig = new AmazonS3Config()
            {
                ServiceURL = configuration.ServerUrl,
                SignatureVersion = configuration.SignatureVersion,
                ForcePathStyle = configuration.ForcePathStyle,
            };
            return new AmazonS3Client(configuration.AccessKeyId, configuration.SecretAccessKey, clientConfig);
        }
    }
}
