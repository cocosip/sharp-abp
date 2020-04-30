using Amazon.S3;
using AmazonKS3;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace SharpAbp.FileSystem.S3
{

    /// <summary>默认S3客户端工厂
    /// </summary>
    public class DefaultS3ClientFactory : IS3ClientFactory
    {
        private readonly ILogger _logger;

        //S3客户端集合
        private readonly ConcurrentDictionary<S3ClientIdentifier, IAmazonS3> _clientDict;

        public DefaultS3ClientFactory(ILogger<DefaultS3ClientFactory> logger)
        {
            _logger = logger;
            _clientDict = new ConcurrentDictionary<S3ClientIdentifier, IAmazonS3>();
        }

        /// <summary>根据Bucket信息获取S3客户端
        /// </summary>
        public IAmazonS3 GetClientByBucket(BucketInfo bucket)
        {
            var identifier = ConvertToIdentifier(bucket);
            if (_clientDict.TryGetValue(identifier, out IAmazonS3 client))
            {
                return client;
            }
            //不存在该类型的客户端,则新建
            if (bucket.VendorName == S3Vendor.KS3Name)
            {
                var config = new AmazonKS3Config()
                {
                    ServiceURL = bucket.ServerUrl,
                    ForcePathStyle = bucket.ForcePathStyle,
                    SignatureVersion = bucket.SignatureVersion,
                };
                client = new AmazonKS3Client(bucket.AccessKeyId, bucket.SecretAccessKey, config);
            }
            else
            {
                var config = new AmazonS3Config()
                {
                    ServiceURL = bucket.ServerUrl,
                    ForcePathStyle = bucket.ForcePathStyle,
                    SignatureVersion = bucket.SignatureVersion,
                };
                client = new AmazonS3Client(bucket.AccessKeyId, bucket.SecretAccessKey, config);
            }
            //添加到集合中
            _clientDict.AddOrUpdate(identifier, client, (i, c) => { return client; });
            return client;
        }

        /// <summary>根据Bucket信息转换成客户端标志
        /// </summary>
        private S3ClientIdentifier ConvertToIdentifier(BucketInfo bucket)
        {
            return new S3ClientIdentifier(bucket.AccessKeyId, bucket.SecretAccessKey, new S3Vendor(bucket.VendorName, bucket.VendorVersion));
            //return new S3ClientIdentifier(bucket.AccessKeyId, bucket.SecretAccessKey, bucket.Vendor);
        }



    }
}
