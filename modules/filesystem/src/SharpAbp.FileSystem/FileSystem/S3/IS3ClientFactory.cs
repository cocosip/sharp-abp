using Amazon.S3;

namespace SharpAbp.FileSystem.S3
{

    /// <summary>S3客户端策略工厂
    /// </summary>
    public interface IS3ClientFactory
    {
        /// <summary>根据Bucket信息获取S3客户端
        /// </summary>
        IAmazonS3 GetClientByBucket(BucketInfo bucket);
    }
}
