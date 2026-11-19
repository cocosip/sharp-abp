using System.Data;
using Moq;
using OceanBaseConnector;
using SharpAbp.Abp.Data.SqlBuilder.OceanBase;
using Xunit;

namespace SharpAbp.Abp.Data.SqlBuilder
{
    /// <summary>
    /// Unit tests for the OceanBase connector specific mode detector
    /// </summary>
    public class OceanBaseSpecificDatabaseModeDetectorTests
    {
        [Fact]
        public void DetectMode_UnopenedOceanBaseConnection_ReturnsOracle()
        {
            using var connection = new OceanBaseConnection();
            var detector = new OceanBaseDatabaseModeDetector();

            var result = detector.DetectMode(connection);

            Assert.Equal(OceanBaseDatabaseMode.Oracle, result);
        }

        [Fact]
        public void DetectMode_OtherConnection_ReturnsOracle()
        {
            var detector = new OceanBaseDatabaseModeDetector();
            var mockConnection = new Mock<IDbConnection>();

            var result = detector.DetectMode(mockConnection.Object);

            Assert.Equal(OceanBaseDatabaseMode.Oracle, result);
        }

        [Fact]
        public void DetectMode_NullConnection_ReturnsOracle()
        {
            var detector = new OceanBaseDatabaseModeDetector();

            var result = detector.DetectMode(null);

            Assert.Equal(OceanBaseDatabaseMode.Oracle, result);
        }
    }
}
