namespace SharpAbp.Abp.FileStoring.S3
{
    public static class S3FileProviderConfigurationNames
    {
        public const string ProviderName = "S3";

        public const string Region = "S3.Region";
        public const string BucketName = "S3.BucketName";
        public const string ServerUrl = "S3.ServerUrl";
        public const string AccessKeyId = "S3.AccessKeyId";
        public const string SecretAccessKey = "S3.SecretAccessKey";
        public const string ForcePathStyle = "S3.ForcePathStyle";
        public const string UseChunkEncoding = "S3.UseChunkEncoding";

        //1-HTTP,2-HTTPS
        public const string Protocol = "S3.Protocol";
        //1-Amazon, 2-KS3,4-Other
        //public const string VendorType = "S3.VendorType";
        public const string SignatureVersion = "S3.SignatureVersion";
        public const string CreateBucketIfNotExists = "S3.CreateBucketIfNotExists";

        public const string DefaultRegion = "Region";
    }
}
