using System;
using Xunit;

namespace SharpAbp.MinId;

public class MinIdMapperlyTest
{
    [Fact]
    public void Configuration_Maps_All_Allocation_Parameters_To_Dto_And_Cache()
    {
        var source = new MinIdInfo(Guid.NewGuid(), "orders", 1234, 100, 3, 2);
        var dtoMapper = new MinIdInfoToMinIdInfoDtoMapper();
        var cacheMapper = new MinIdInfoToMinIdInfoCacheItemMapper();
        var dto = new MinIdInfoDto();
        var cache = new MinIdInfoCacheItem();

        dtoMapper.Map(source, dto);
        cacheMapper.Map(source, cache);

        foreach (var result in new[] { dtoMapper.Map(source), dto })
        {
            Assert.Equal(source.Id, result.Id);
            Assert.Equal("orders", result.BizType);
            Assert.Equal(1234, result.MaxId);
            Assert.Equal(100, result.Step);
            Assert.Equal(3, result.Delta);
            Assert.Equal(2, result.Remainder);
        }
        foreach (var result in new[] { cacheMapper.Map(source), cache })
        {
            Assert.Equal("orders", result.BizType);
            Assert.Equal(1234, result.MaxId);
            Assert.Equal(100, result.Step);
            Assert.Equal(3, result.Delta);
            Assert.Equal(2, result.Remainder);
        }
    }

    [Fact]
    public void Token_Maps_Business_Type_And_Value_To_Dto_And_Cache()
    {
        var source = new MinIdToken(Guid.NewGuid(), "orders", "token123", "description");
        var dtoMapper = new MinIdTokenToMinIdTokenDtoMapper();
        var cacheMapper = new MinIdTokenToMinIdTokenCacheItemMapper();
        var dto = new MinIdTokenDto();
        var cache = new MinIdTokenCacheItem();

        dtoMapper.Map(source, dto);
        cacheMapper.Map(source, cache);

        foreach (var result in new[] { dtoMapper.Map(source), dto })
        {
            Assert.Equal(source.Id, result.Id);
            Assert.Equal("orders", result.BizType);
            Assert.Equal("token123", result.Token);
            Assert.Equal("description", result.Remark);
        }
        foreach (var result in new[] { cacheMapper.Map(source), cache })
        {
            Assert.Equal("orders", result.BizType);
            Assert.Equal("token123", result.Token);
        }
    }

    [Fact]
    public void Segment_Maps_Current_Position_And_Remainder_With_Both_Overloads()
    {
        var source = new SegmentId { CurrentId = 71, MaxId = 100, Delta = 3, Remainder = 2, LoadingId = 80 };
        var mapper = new SegmentIdToSegmentIdDtoMapper();
        var destination = new SegmentIdDto();

        mapper.Map(source, destination);

        foreach (var result in new[] { mapper.Map(source), destination })
        {
            Assert.Equal(71, result.CurrentId);
            Assert.Equal(100, result.MaxId);
            Assert.Equal(3, result.Delta);
            Assert.Equal(2, result.Remainder);
            Assert.Equal(80, result.LoadingId);
        }
    }
}
