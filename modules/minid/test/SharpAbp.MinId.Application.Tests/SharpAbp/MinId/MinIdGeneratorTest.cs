using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SharpAbp.MinId
{
    public class MinIdGeneratorTest : MinIdApplicationTestBase
    {
        private readonly IMinIdGeneratorFactory _minIdGeneratorFactory;
        public MinIdGeneratorTest()
        {
            _minIdGeneratorFactory = GetRequiredService<IMinIdGeneratorFactory>();
        }


        [Fact]
        public async Task NextId_Test_Async()
        {
            var minIdGenerator = await _minIdGeneratorFactory.GetAsync("default");
            var id1 = await minIdGenerator.NextIdAsync();
            Assert.True(id1 > 0);

            var ids = await minIdGenerator.NextIdAsync(10);
            Assert.True(ids.All(x => x > id1));
            Assert.Equal(10, ids.Count);
        }


 

    }
}
