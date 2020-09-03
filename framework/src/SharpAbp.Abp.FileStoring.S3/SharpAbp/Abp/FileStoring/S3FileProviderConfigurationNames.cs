namespace SharpAbp.Abp.FileStoring
{
    public static class S3FileProviderConfigurationNames
    {
        public const string ConnectionString = "S3.ConnectionString";
        public const string AccessKeyId = "S3.AccessKeyId";
        public const string AccessKeySecret = "S3.AccessKeySecret";
        public const string ForcePathStyle = "S3.ForcePathStyle";
        public const string UseChunkEncoding = "S3.UseChunkEncoding";

        //1-HTTP,2-HTTPS
        public const string Protocol = "S3.Protocol";
        public const string Vendor = "S3.Vendor";
        public const string EnableSlice = "S3.EnableSlice";
        public const string SliceSize = "S3.SliceSize";
        public const string SignatureVersion = "S3.SignatureVersion";


        public const string WithSSL = "S3.WithSSL";
        public const string ContainerName = "S3.ContainerName";
        public const string CreateContainerIfNotExists = "S3.CreateContainerIfNotExists";
    }
}
