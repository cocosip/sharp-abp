using Xunit;

namespace SharpAbp.MinId;

public class MinIdUtilTest
{
    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData(" ", false)]
    [InlineData("ab", false)]
    [InlineData("Ab9", true)]
    [InlineData("abc-def", false)]
    [InlineData("abc_def", false)]
    [InlineData("中文字符", false)]
    public void Identifiers_Require_At_Least_Three_Ascii_Alphanumeric_Characters(string input, bool valid)
    {
        Assert.Equal(valid, MinIdUtil.IsToken(input));
        Assert.Equal(valid, MinIdUtil.IsBizType(input));
    }

    [Fact]
    public void Token_And_Business_Type_Enforce_Their_Own_Maximum_Length()
    {
        Assert.True(MinIdUtil.IsToken(new string('a', 40)));
        Assert.False(MinIdUtil.IsToken(new string('a', 41)));
        Assert.True(MinIdUtil.IsBizType(new string('a', 32)));
        Assert.False(MinIdUtil.IsBizType(new string('a', 33)));
    }
}
