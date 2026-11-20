using System;
using Xunit;

namespace SharpAbp.Abp.Snowflakes;

public class SnowflakeNameAttributeTest
{
    [Fact]
    public void Unnamed_Type_Should_Require_An_Explicit_Name()
    {
        Assert.Throws<ArgumentException>(() => SnowflakeNameAttribute.GetSnowflakeName(typeof(Generic<>).GetGenericArguments()[0]));
    }

    [Fact]
    public void Ordinary_Type_Should_Use_The_Full_Name()
    {
        Assert.Equal(typeof(Generic<>).FullName, SnowflakeNameAttribute.GetSnowflakeName(typeof(Generic<>)));
    }

    [Fact]
    public void Explicit_Name_Should_Be_Preserved()
    {
        Assert.Equal("orders", SnowflakeNameAttribute.GetSnowflakeName<Named>());
    }

    private class Generic<T> { }

    [SnowflakeName("orders")]
    private class Named { }
}
