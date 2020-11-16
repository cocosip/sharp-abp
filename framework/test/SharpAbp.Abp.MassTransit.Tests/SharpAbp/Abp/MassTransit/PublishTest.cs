using MassTransit;
using SharpAbp.Abp.MassTransit.TestObjects;
using System.Threading.Tasks;
using Xunit;

namespace SharpAbp.Abp.MassTransit
{
    public class PublishTest : AbpMassTransitTestBase
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public PublishTest()
        {
            _publishEndpoint = GetService<IPublishEndpoint>();
        }

        [Fact]
        public async Task Publish_Message_Test()
        {
            await _publishEndpoint.Publish<TestMessage1>(new TestMessage1()
            {
                Name = "zhangsan"
            });
        }

    }
}
