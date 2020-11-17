using MassTransit;
using SharpAbp.Abp.MassTransit.TestObjects;
using System.Threading.Tasks;
using Xunit;

namespace SharpAbp.Abp.MassTransit
{
    public class PublishSubscribeTest : AbpMassTransitTestBase
    {
        public static int ConsumerCount = 0;

        private readonly IPublishEndpoint _publishEndpoint;
        public PublishSubscribeTest()
        {
            _publishEndpoint = GetRequiredService<IPublishEndpoint>();
        }

        [Fact]
        public async Task Publish_Message_Test()
        {
            await _publishEndpoint.Publish(new OrderSubmitted()
            {
                OrderId = "1"
            });


            //await Task.Delay(100);

           // Assert.True(ConsumerCount > 0);
        }

    }
}
