using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Xunit;

namespace SharpAbp.Abp.FileStoring.Obs
{
    public class ObsFileProviderValuesValidatorTest : AbpFileStoringAllTestBase
    {
        private List<NameValue> CreateValues(string authType, string authTypeNegotiation)
        {
            return
            [
                new(ObsFileProviderConfigurationNames.Endpoint, "http://127.0.0.1:9000"),
                new(ObsFileProviderConfigurationNames.BucketName, "bucket"),
                new(ObsFileProviderConfigurationNames.AccessKeyId, "AccessKeyId"),
                new(ObsFileProviderConfigurationNames.AccessKeySecret, "AccessKeySecret"),
                new(ObsFileProviderConfigurationNames.AuthType, authType),
                new(ObsFileProviderConfigurationNames.AuthTypeNegotiation, authTypeNegotiation),
                new(ObsFileProviderConfigurationNames.CreateContainerIfNotExists, "false")
            ];
        }

        private IFileProviderValuesValidator GetValidator()
        {
            return GetRequiredService<IServiceProvider>()
                .GetRequiredKeyedService<IFileProviderValuesValidator>(ObsFileProviderConfigurationNames.ProviderName);
        }

        [Fact]
        public void Validate_Should_Allow_Default_Auth_Type_With_Negotiation()
        {
            var result = GetValidator().Validate(CreateValues("OBS", "true"));

            Assert.Empty(result.Errors);
        }

        [Fact]
        public void Validate_Should_Allow_V4_Without_Negotiation()
        {
            var result = GetValidator().Validate(CreateValues("v4", "false"));

            Assert.Empty(result.Errors);
        }

        [Fact]
        public void Validate_Should_Reject_Unknown_Auth_Type()
        {
            var result = GetValidator().Validate(CreateValues("v5", "false"));

            Assert.NotEmpty(result.Errors);
        }

        [Fact]
        public void Validate_Should_Reject_Auth_Type_With_Negotiation()
        {
            var result = GetValidator().Validate(CreateValues("V4", "true"));

            Assert.NotEmpty(result.Errors);
        }
    }
}
