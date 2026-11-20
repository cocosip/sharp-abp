using System.Diagnostics;
using System.Reflection;
using SharpAbp.Abp.Core;
using Xunit;

namespace SharpAbp.Abp.FileStoringManagement;

public class BuildConfigurationTest
{
    [Fact]
    public void Framework_Project_References_Should_Use_The_Requested_Build_Configuration()
    {
        var debugging = typeof(NumberCheck).Assembly.GetCustomAttribute<DebuggableAttribute>();
        Assert.NotNull(debugging);
#if DEBUG
        Assert.True(debugging.IsJITOptimizerDisabled);
#else
        Assert.False(debugging.IsJITOptimizerDisabled);
#endif
    }
}
