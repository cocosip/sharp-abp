using System.Data;
using Moq;
using Xunit;

namespace SharpAbp.Abp.Data.SqlBuilder
{
    /// <summary>
    /// Unit tests for DefaultOceanBaseDatabaseModeDetector
    /// </summary>
    public class DefaultOceanBaseDatabaseModeDetectorTests : AbpDataSqlBuilderTestBase
    {
        [Fact]
        public void DetectMode_ReturnsOracleAsDefault()
        {
            var detector = GetRequiredService<IOceanBaseDatabaseModeDetector>();
            var mockConnection = new Mock<IDbConnection>();

            var result = detector.DetectMode(mockConnection.Object);

            Assert.Equal(OceanBaseDatabaseMode.Oracle, result);
        }

        [Fact]
        public void DetectMode_NullConnection_ReturnsOracleAsDefault()
        {
            var detector = GetRequiredService<IOceanBaseDatabaseModeDetector>();

            var result = detector.DetectMode(null);

            Assert.Equal(OceanBaseDatabaseMode.Oracle, result);
        }
    }
}
