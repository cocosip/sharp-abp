using System;
using System.Data;
using OceanBaseConnector;
using SharpAbp.Abp.Dapper.OceanBase;
using Xunit;

namespace SharpAbp.Abp.Dapper
{
    public sealed class OceanBaseValueTypeMappingTests
    {
        private static readonly DateTime Instant = new(2024, 1, 2, 3, 4, 5, DateTimeKind.Unspecified);

        [Fact]
        public void Decimal_Mapping_Should_Read_OceanBaseDecimal()
        {
            var mapping = new OceanBaseDecimalTypeMapping();
            var value = new OceanBaseDecimal(1234.5678m);

            Assert.True(mapping.CanParse(value));
            Assert.Equal(1234.5678m, mapping.Parse(value));

            Assert.False(mapping.CanParse(1234.5678m));
            Assert.False(mapping.CanParse(1234));
            Assert.False(mapping.CanParse("1234.5678"));
            Assert.False(mapping.CanParse(DBNull.Value));
        }

        [Fact]
        public void Timestamp_Mapping_Should_Read_OceanBaseTimestamp()
        {
            var mapping = new OceanBaseTimestampTypeMapping();
            var value = new OceanBaseTimestamp(Instant, 123456700, 9);

            Assert.True(mapping.CanParse(value));
            Assert.Equal(Instant.AddTicks(1234567), mapping.Parse(value));

            var exact = new OceanBaseTimestamp(Instant, 0, 9);
            Assert.Equal(Instant, mapping.Parse(exact));
            Assert.Throws<OverflowException>(() => mapping.Parse(new OceanBaseTimestamp(Instant, 1, 9)));
        }

        [Fact]
        public void Timestamp_Mapping_Should_Read_Local_Time_Zone_Timestamps()
        {
            var mapping = new OceanBaseTimestampTypeMapping();
            var value = new OceanBaseTimestampLocalTimeZone(new OceanBaseTimestamp(Instant), "+08:00");

            Assert.True(mapping.CanParse(value));
            Assert.Equal(Instant, mapping.Parse(value));
        }

        [Fact]
        public void Timestamp_With_Time_Zone_Mapping_Should_Read_OceanBaseTimestampWithTimeZone()
        {
            var mapping = new OceanBaseTimestampWithTimeZoneTypeMapping();
            var expected = new DateTimeOffset(2024, 1, 2, 3, 4, 5, TimeSpan.FromHours(8));
            var value = new OceanBaseTimestampWithTimeZone(expected);

            Assert.True(mapping.CanParse(value));
            Assert.Equal(expected, mapping.Parse(value));

            Assert.False(mapping.CanParse(new OceanBaseTimestamp(Instant)));
            Assert.False(mapping.CanParse(expected));
        }

        [Fact]
        public void Value_Type_Mappings_Should_Be_Mutually_Exclusive()
        {
            var timestamp = new OceanBaseTimestamp(Instant);
            var timestampMapping = new OceanBaseTimestampTypeMapping();

            Assert.False(timestampMapping.CanParse(new OceanBaseDecimal(1m)));
            Assert.False(timestampMapping.CanParse(new OceanBaseTimestampWithTimeZone(timestamp, TimeSpan.Zero)));
            Assert.False(new OceanBaseDecimalTypeMapping().CanParse(timestamp));
        }

        [Fact]
        public void Value_Type_Mappings_Should_Leave_Writing_To_The_Driver()
        {
            IDapperTypeMapping[] mappings =
            [
                new OceanBaseDecimalTypeMapping(),
                new OceanBaseTimestampTypeMapping(),
                new OceanBaseTimestampWithTimeZoneTypeMapping()
            ];

            foreach (var mapping in mappings)
            {
                Assert.False(mapping.CanSetValue(OceanBaseTestParameters.Create(OceanBaseCompatibilityMode.Oracle)));
                Assert.False(mapping.CanSetValue(OceanBaseTestParameters.Create(OceanBaseCompatibilityMode.MySql)));
                Assert.False(mapping.CanSetValue(new TestDbParameter()));
            }

            Assert.Throws<NotSupportedException>(
                () => new OceanBaseDecimalTypeMapping().SetValue(new TestDbParameter(), 1m));
            Assert.Throws<NotSupportedException>(
                () => new OceanBaseTimestampTypeMapping().SetValue(new TestDbParameter(), Instant));
            Assert.Throws<NotSupportedException>(
                () => new OceanBaseTimestampWithTimeZoneTypeMapping()
                    .SetValue(new TestDbParameter(), DateTimeOffset.UnixEpoch));
        }
    }
}
