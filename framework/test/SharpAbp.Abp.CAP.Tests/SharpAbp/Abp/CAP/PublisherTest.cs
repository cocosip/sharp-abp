using DotNetCore.CAP;
using System.Threading.Tasks;
using Xunit;

namespace SharpAbp.Abp.CAP
{
    public class PublisherTest : AbpCapTestBase
    {
        private readonly ICapPublisher _capPublisher;

        public PublisherTest()
        {
            _capPublisher = GetRequiredService<ICapPublisher>();
        }

        [Fact]
        public async Task Publish_Message_Async_Test()
        {
            await Task.FromResult(1);
            //await _capPublisher.PublishAsync("topic1", "123456");
        }
    }
}
