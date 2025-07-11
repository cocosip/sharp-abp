﻿using Amazon.S3;
using System.Threading.Tasks;

namespace SharpAbp.Abp.FileStoring.Aws
{
    public interface IAmazonS3ClientFactory
    {
        Task<IAmazonS3> GetAmazonS3Client(AwsFileProviderConfiguration configuration);
    }
}
