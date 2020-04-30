using System;

namespace SharpAbp.FileSystem.S3
{
    /// <summary>S3存储下的Patch
    /// </summary>
    [Serializable]
    public class S3Patch : Patch
    {
        /// <summary>Bucket信息
        /// </summary>
        public BucketInfo Bucket { get; set; }

        /// <summary>Ctor
        /// </summary>
        public S3Patch()
        {
            StoreType = (int) SharpAbp.FileSystem.StoreType.S3;
        }


    }
}
