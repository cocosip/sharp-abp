using System;
using Xunit;

namespace SharpAbp.Abp.DbConnections
{
    public class DbConnectionNameAttributeTest
    {
        [Fact]
        public void GetDbConnectionName_Should_Return_Attribute_Name_When_Present()
        {
            var name = DbConnectionNameAttribute.GetDbConnectionName(typeof(CustomNamedDbConnection));

            Assert.Equal("custom", name);
        }

        [Fact]
        public void GetDbConnectionName_Should_Return_Full_Type_Name_When_Attribute_Is_Missing()
        {
            var name = DbConnectionNameAttribute.GetDbConnectionName(typeof(DefaultNamedDbConnection));

            Assert.Equal(typeof(DefaultNamedDbConnection).FullName, name);
        }

        [Fact]
        public void Constructor_Should_Validate_Name()
        {
            Assert.Throws<ArgumentException>(() => new DbConnectionNameAttribute(string.Empty));
        }

        [DbConnectionName("custom")]
        private class CustomNamedDbConnection
        {
        }

        private class DefaultNamedDbConnection
        {
        }
    }
}
