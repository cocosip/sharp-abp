using Dm;
using Microsoft.Extensions.Options;
using SharpAbp.Abp.Data.SqlBuilder.DM;
using SharpAbp.Abp.EntityFrameworkCore;
using System.Collections.Generic;
using Xunit;

namespace SharpAbp.Abp.Data.SqlBuilder
{
    public class DmSpecificDatabaseModeDetectorTests : AbpDataSqlBuilderTestBase
    {
        [Theory]
        [InlineData(DmDatabaseMode.Oracle)]
        [InlineData(DmDatabaseMode.PostgreSql)]
        [InlineData(DmDatabaseMode.MySql)]
        public void DetectMode_ShouldReturnConfiguredMode_WhenDmModeConfigured(DmDatabaseMode configuredMode)
        {
            // Arrange
            var detector = CreateDetector(configuredMode.ToString());
            var connection = new DmConnection();

            // Act
            var result = detector.DetectMode(connection);

            // Assert
            Assert.Equal(configuredMode, result);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("SqlServer")]
        public void DetectMode_ShouldFallBackToExistingDefault_WhenConfiguredModeIsInvalidOrEmpty(string configuredMode)
        {
            // Arrange
            var detector = CreateDetector(configuredMode);

            // Act
            var result = detector.DetectMode(null);

            // Assert
            Assert.Equal(DmDatabaseMode.Oracle, result);
        }

        private static DmDatabaseModeDetector CreateDetector(string configuredMode)
        {
            var options = new SharpAbpEfCoreOptions();

            if (configuredMode != null)
            {
                options.Properties = new Dictionary<string, string>
                {
                    { EfCoreConstants.PropertyNames.DmDatabaseMode, configuredMode }
                };
            }

            return new DmDatabaseModeDetector(Options.Create(options));
        }
    }
}
