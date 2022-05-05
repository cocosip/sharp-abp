using Microsoft.Extensions.Options;
using SharpAbp.Abp.FileStoring.Aliyun;
using SharpAbp.Abp.FileStoring.Azure;
using SharpAbp.Abp.FileStoring.FastDFS;
using SharpAbp.Abp.FileStoring.FileSystem;
using SharpAbp.Abp.FileStoring.Minio;
using SharpAbp.Abp.FileStoring.S3;
using System;
using System.IO;
using Xunit;

namespace SharpAbp.Abp.FileStoring
{
    public class AbpFileStoringOptionsTest : AbpFileStoringAllTestBase
    {
        private readonly IFileContainerConfigurationProvider _configurationProvider;
        private readonly AbpFileStoringOptions _options;
        private readonly IFileContainerFactory _fileContainerFactory;
        public AbpFileStoringOptionsTest()
        {
            _configurationProvider = GetRequiredService<IFileContainerConfigurationProvider>();
            _options = GetRequiredService<IOptions<AbpFileStoringOptions>>().Value;
            _fileContainerFactory = GetRequiredService<IFileContainerFactory>();
        }


        [Fact]
        public void Aliyun_Default_Configuration_Test()
        {
            var configuration = _configurationProvider.Get<DefaultContainer>();
            Assert.Equal("Aliyun", configuration.Provider);
            Assert.False(configuration.IsMultiTenant);
            Assert.True(configuration.HttpAccess);

            var aliyunConfiguration = configuration.GetAliyunConfiguration();
            Assert.Equal("oss-cn-hangzhou", aliyunConfiguration.RegionId);
            Assert.Equal("oss-cn-hangzhou.aliyuncs.com", aliyunConfiguration.Endpoint);
            Assert.Equal("aliyun-bucket", aliyunConfiguration.BucketName);
            Assert.Equal("AccessKeyId", aliyunConfiguration.AccessKeyId);
            Assert.Equal("AccessKeySecret", aliyunConfiguration.AccessKeySecret);
            Assert.False(aliyunConfiguration.UseSecurityTokenService);
            Assert.True(aliyunConfiguration.RoleArn.IsNullOrWhiteSpace());
            Assert.True(aliyunConfiguration.RoleSessionName.IsNullOrWhiteSpace());
            Assert.Equal(100, aliyunConfiguration.DurationSeconds);
            Assert.True(aliyunConfiguration.Policy.IsNullOrWhiteSpace());
            Assert.True(aliyunConfiguration.CreateContainerIfNotExists);
            Assert.Equal("key1", aliyunConfiguration.TemporaryCredentialsCacheKey);

        }

        [Fact]
        public void Azure_Configuration_Test()
        {
            var configuration = _configurationProvider.Get("azure-container");
            Assert.Equal("Azure", configuration.Provider);
            Assert.False(configuration.IsMultiTenant);
            Assert.False(configuration.HttpAccess);

            var azureConfiguration = configuration.GetAzureConfiguration();

            Assert.Equal("connection1", azureConfiguration.ConnectionString);
            Assert.Equal("azure-container", azureConfiguration.ContainerName);
            Assert.False(azureConfiguration.CreateContainerIfNotExists);
        }

        [Fact]
        public void FastDFS_Configuration_Test()
        {
            var configuration = _configurationProvider.Get("fastdfs-container");
            Assert.Equal("FastDFS", configuration.Provider);
            Assert.True(configuration.IsMultiTenant);
            Assert.True(configuration.HttpAccess);

            var fastDFSConfiguration = configuration.GetFastDFSConfiguration();

            Assert.Equal("default", fastDFSConfiguration.ClusterName);
            Assert.Equal("http://192.168.0.100", fastDFSConfiguration.HttpServer);
            Assert.Equal("group1", fastDFSConfiguration.GroupName);
            Assert.True(fastDFSConfiguration.AppendGroupNameToUrl);
            Assert.Equal("192.168.0.101:22122,192.168.0.102:22122", fastDFSConfiguration.Trackers);
            Assert.True(fastDFSConfiguration.AntiStealCheckToken);
            Assert.Equal("123456", fastDFSConfiguration.SecretKey);
            Assert.Equal("utf-8", fastDFSConfiguration.Charset);
            Assert.Equal(300, fastDFSConfiguration.ConnectionTimeout);
            Assert.Equal(600, fastDFSConfiguration.ConnectionLifeTime);
            Assert.Equal(1, fastDFSConfiguration.ConnectionConcurrentThread);
            Assert.Equal(100, fastDFSConfiguration.ScanTimeoutConnectionInterval);
            Assert.Equal(10, fastDFSConfiguration.TrackerMaxConnection);
            Assert.Equal(30, fastDFSConfiguration.StorageMaxConnection);

        }

        [Fact]
        public void FileSystem_Configuration_Test()
        {
            var configuration = _configurationProvider.Get("filesystem-container");
            Assert.Equal("FileSystem", configuration.Provider);
            Assert.False(configuration.IsMultiTenant);
            Assert.False(configuration.HttpAccess);

            var fileSystemConfiguration = configuration.GetFileSystemConfiguration();

            Assert.Equal("/usr/local/file_system", fileSystemConfiguration.BasePath);
            Assert.True(fileSystemConfiguration.AppendContainerNameToBasePath);
            Assert.Equal("http://192.168.0.100", fileSystemConfiguration.HttpServer);
        }

        [Fact]
        public void Minio_Configuration_Test()
        {
            var configuration = _configurationProvider.Get("minio-container");
            Assert.Equal("Minio", configuration.Provider);
            Assert.True(configuration.IsMultiTenant);
            Assert.True(configuration.HttpAccess);

            var minioConfiguration = configuration.GetMinioConfiguration();

            Assert.Equal("minio-bucket", minioConfiguration.BucketName);
            Assert.Equal("endpoint1", minioConfiguration.EndPoint);
            Assert.Equal("AccessKey", minioConfiguration.AccessKey);
            Assert.Equal("SecretKey", minioConfiguration.SecretKey);
            Assert.False(minioConfiguration.WithSSL);
            Assert.True(minioConfiguration.CreateBucketIfNotExists);


            //var container = _fileContainerFactory.Create("minio-test");
            //var fileId = await container.SaveAsync("2022/04/18/1.dcm", "D:\\1.dcm", true);
            //await container.DownloadAsync(fileId, "D:\\2.dcm");
        }


        [Fact]
        public void S3_Configuration_Test()
        {
            var configuration = _configurationProvider.Get("s3-container");
            Assert.Equal("S3", configuration.Provider);
            Assert.True(configuration.IsMultiTenant);
            Assert.True(configuration.HttpAccess);

            var s3Configuration = configuration.GetS3Configuration();

            Assert.Equal("s3-bucket", s3Configuration.BucketName);
            Assert.Equal("http://192.168.0.100", s3Configuration.ServerUrl);
            Assert.Equal("AccessKeyId", s3Configuration.AccessKeyId);
            Assert.Equal("SecretAccessKey", s3Configuration.SecretAccessKey);
            Assert.False(s3Configuration.ForcePathStyle);
            Assert.False(s3Configuration.UseChunkEncoding);
            Assert.Equal(0, s3Configuration.Protocol); //0-HTTPS,1-HTTP
            Assert.True(s3Configuration.EnableSlice);
            Assert.Equal(5242880, s3Configuration.SliceSize);
            Assert.Equal("2.0", s3Configuration.SignatureVersion);
            Assert.False(s3Configuration.CreateBucketIfNotExists);
        }

        [Fact]
        public void FileProviders_Test()
        {
            var fileProviderConfigurations = _options.Providers.GetFileProviders();
            Assert.Equal(9, fileProviderConfigurations.Count);

            var aliyunProviderConfiguration = _options.Providers.GetConfiguration("Aliyun");
            Assert.Equal("Aliyun", aliyunProviderConfiguration.Provider);

            var obsProviderConfiguration = _options.Providers.GetConfiguration("Obs");
            Assert.Equal("Obs", obsProviderConfiguration.Provider);

            var azureProviderConfiguration = _options.Providers.GetConfiguration("Azure");
            Assert.Equal("Azure", azureProviderConfiguration.Provider);

            var fastDFSProviderConfiguration = _options.Providers.GetConfiguration("FastDFS");
            Assert.Equal("FastDFS", fastDFSProviderConfiguration.Provider);

            var minioProviderConfiguration = _options.Providers.GetConfiguration("Minio");
            Assert.Equal("Minio", minioProviderConfiguration.Provider);

            var fileSystemProviderConfiguration = _options.Providers.GetConfiguration("FileSystem");
            Assert.Equal("FileSystem", fileSystemProviderConfiguration.Provider);

            var awsProviderConfiguration = _options.Providers.GetConfiguration("Aws");
            Assert.Equal("Aws", awsProviderConfiguration.Provider);

            var ks3ProviderConfiguration = _options.Providers.GetConfiguration("KS3");
            Assert.Equal("KS3", ks3ProviderConfiguration.Provider);

            var s3ProviderConfiguration = _options.Providers.GetConfiguration("S3");
            Assert.Equal("S3", s3ProviderConfiguration.Provider);

        }

    }
}
