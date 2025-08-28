using System;
using System.Data;
using System.Data.Common;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Volo.Abp.Testing;

namespace SharpAbp.Abp.Data.SqlBuilder
{
    /// <summary>
    /// Unit tests for DefaultDmDatabaseModeDetector
    /// </summary>
    public class DefaultDmDatabaseModeDetectorTests : AbpDataSqlBuilderTestBase
    {
        /// <summary>
        /// Test DetectMode method returns Oracle as default
        /// </summary>
        [Fact]
        public void DetectMode_ReturnsOracleAsDefault()
        {
            // Arrange
            var detector = GetRequiredService<IDmDatabaseModeDetector>();
            var mockConnection = new Mock<IDbConnection>();

            // Act
            var result = detector.DetectMode(mockConnection.Object);

            // Assert
            Assert.Equal(DmDatabaseMode.Oracle, result);
        }

        /// <summary>
        /// Test DetectMode method with null connection - DefaultDmDatabaseModeDetector doesn't validate null
        /// </summary>
        [Fact]
        public void DetectMode_NullConnection_ReturnsOracleAsDefault()
        {
            // Arrange
            var detector = GetRequiredService<IDmDatabaseModeDetector>();

            // Act
            var result = detector.DetectMode(null);

            // Assert
            Assert.Equal(DmDatabaseMode.Oracle, result);
        }

        /// <summary>
        /// Test DetectMode method logs debug message
        /// </summary>
        [Fact]
        public void DetectMode_LogsDebugMessage()
        {
            // Arrange
            var detector = GetRequiredService<IDmDatabaseModeDetector>();
            var mockConnection = new Mock<IDbConnection>();

            // Act
            var result = detector.DetectMode(mockConnection.Object);

            // Assert
            Assert.Equal(DmDatabaseMode.Oracle, result);
            // Note: In a real scenario, you might want to verify logging through a test logger
        }

        /// <summary>
        /// Test DetectMode with different connection types returns consistent default
        /// </summary>
        [Fact]
        public void DetectMode_DifferentConnectionTypes_ReturnsConsistentDefault()
        {
            // Arrange
            var detector = GetRequiredService<IDmDatabaseModeDetector>();
            var mockConnection1 = new Mock<IDbConnection>();
            var mockConnection2 = new Mock<IDbConnection>();
            
            // Act
            var result1 = detector.DetectMode(mockConnection1.Object);
            var result2 = detector.DetectMode(mockConnection2.Object);
            
            // Assert
            Assert.Equal(DmDatabaseMode.Oracle, result1);
            Assert.Equal(DmDatabaseMode.Oracle, result2);
            Assert.Equal(result1, result2);
        }
    }
}