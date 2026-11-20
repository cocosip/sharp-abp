using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp;
using Xunit;
using AliyunNames = SharpAbp.Abp.FileStoring.Aliyun.AliyunFileProviderConfigurationNames;
using AwsNames = SharpAbp.Abp.FileStoring.Aws.AwsFileProviderConfigurationNames;
using AzureNames = SharpAbp.Abp.FileStoring.Azure.AzureFileProviderConfigurationNames;
using FastDfsNames = SharpAbp.Abp.FileStoring.FastDFS.FastDFSFileProviderConfigurationNames;
using FileSystemNames = SharpAbp.Abp.FileStoring.FileSystem.FileSystemFileProviderConfigurationNames;
using Ks3Names = SharpAbp.Abp.FileStoring.KS3.KS3FileProviderConfigurationNames;
using MinioNames = SharpAbp.Abp.FileStoring.Minio.MinioFileProviderConfigurationNames;
using ObsNames = SharpAbp.Abp.FileStoring.Obs.ObsFileProviderConfigurationNames;
using S3Names = SharpAbp.Abp.FileStoring.S3.S3FileProviderConfigurationNames;

namespace SharpAbp.Abp.FileStoring;

public class FileProviderValuesValidationTest : AbpFileStoringAllTestBase
{
    [Theory]
    [InlineData(AliyunNames.ProviderName)]
    [InlineData(AwsNames.ProviderName)]
    [InlineData(AzureNames.ProviderName)]
    [InlineData(FastDfsNames.ProviderName)]
    [InlineData(FileSystemNames.ProviderName)]
    [InlineData(Ks3Names.ProviderName)]
    [InlineData(MinioNames.ProviderName)]
    [InlineData(ObsNames.ProviderName)]
    [InlineData(S3Names.ProviderName)]
    public void Complete_Configuration_Should_Pass_Registered_Validator(string provider)
    {
        var validator = ServiceProvider.GetRequiredKeyedService<IFileProviderValuesValidator>(provider);

        var result = validator.Validate(GetValidValues(provider));

        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData(AliyunNames.ProviderName)]
    [InlineData(AwsNames.ProviderName)]
    [InlineData(AzureNames.ProviderName)]
    [InlineData(FastDfsNames.ProviderName)]
    [InlineData(FileSystemNames.ProviderName)]
    [InlineData(Ks3Names.ProviderName)]
    [InlineData(MinioNames.ProviderName)]
    [InlineData(ObsNames.ProviderName)]
    [InlineData(S3Names.ProviderName)]
    public void Missing_Required_Settings_Should_Report_All_Configuration_Keys(string provider)
    {
        var validator = ServiceProvider.GetRequiredKeyedService<IFileProviderValuesValidator>(provider);

        var result = validator.Validate([]);

        Assert.NotEmpty(result.Errors);
        foreach (var value in GetValidValues(provider))
        {
            Assert.Contains(result.Errors, error => error.ErrorMessage.Contains(value.Name));
        }
    }

    [Theory]
    [InlineData(AliyunNames.ProviderName, AliyunNames.Endpoint, " ")]
    [InlineData(AliyunNames.ProviderName, AliyunNames.UseSecurityTokenService, "yes")]
    [InlineData(AliyunNames.ProviderName, AliyunNames.DurationSeconds, "1.5")]
    [InlineData(AwsNames.ProviderName, AwsNames.AccessKeyId, "")]
    [InlineData(AwsNames.ProviderName, AwsNames.CreateContainerIfNotExists, "1")]
    [InlineData(AwsNames.ProviderName, AwsNames.MaxConnectionsPerServer, "many")]
    [InlineData(AzureNames.ProviderName, AzureNames.ConnectionString, " ")]
    [InlineData(AzureNames.ProviderName, AzureNames.CreateContainerIfNotExists, "yes")]
    [InlineData(FastDfsNames.ProviderName, FastDfsNames.Trackers, "")]
    [InlineData(FastDfsNames.ProviderName, FastDfsNames.AntiStealCheckToken, "1")]
    [InlineData(FastDfsNames.ProviderName, FastDfsNames.NetworkTimeout, "seconds")]
    [InlineData(FileSystemNames.ProviderName, FileSystemNames.BasePath, " ")]
    [InlineData(FileSystemNames.ProviderName, FileSystemNames.AppendContainerNameToBasePath, "yes")]
    [InlineData(Ks3Names.ProviderName, Ks3Names.Endpoint, " ")]
    [InlineData(Ks3Names.ProviderName, Ks3Names.CreateContainerIfNotExists, "yes")]
    [InlineData(MinioNames.ProviderName, MinioNames.EndPoint, " ")]
    [InlineData(MinioNames.ProviderName, MinioNames.WithSSL, "1")]
    [InlineData(ObsNames.ProviderName, ObsNames.BucketName, " ")]
    [InlineData(ObsNames.ProviderName, ObsNames.CreateContainerIfNotExists, "yes")]
    [InlineData(S3Names.ProviderName, S3Names.BucketName, " ")]
    [InlineData(S3Names.ProviderName, S3Names.UseChunkEncoding, "yes")]
    [InlineData(S3Names.ProviderName, S3Names.Protocol, "https")]
    [InlineData(S3Names.ProviderName, S3Names.Protocol, "1\n")]
    public void Invalid_Provider_Setting_Should_Identify_The_Field(string provider, string field, string invalidValue)
    {
        var validator = ServiceProvider.GetRequiredKeyedService<IFileProviderValuesValidator>(provider);
        var values = GetValidValues(provider);
        values.Single(value => value.Name == field).Value = invalidValue;

        var result = validator.Validate(values);

        Assert.Contains(result.Errors, error => error.ErrorMessage.Contains(field));
    }

    private List<NameValue> GetValidValues(string provider)
    {
        var values = GetRequiredService<IOptions<AbpFileStoringAbstractionsOptions>>().Value.Providers
            .GetConfiguration(provider).GetItems().Select(item => new NameValue(item.Key, item.Value.Eg)).ToList();
        foreach (var value in values)
        {
            if (value.Name == Ks3Names.AccessKey || value.Name == Ks3Names.SecretKey ||
                value.Name == ObsNames.AccessKeyId || value.Name == ObsNames.AccessKeySecret ||
                value.Name == S3Names.AccessKeyId || value.Name == S3Names.SecretAccessKey)
            {
                value.Value = "test-credential";
            }
            else if (value.Name == S3Names.Protocol)
            {
                value.Value = "0";
            }
        }
        return values;
    }
}
