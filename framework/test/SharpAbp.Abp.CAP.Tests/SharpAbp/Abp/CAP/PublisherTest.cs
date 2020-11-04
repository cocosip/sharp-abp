using DotNetCore.CAP;
using System;
using System.Threading.Tasks;
using Xunit;

namespace SharpAbp.Abp.CAP
{
    public class PublisherTest : AbpCapTestBase
    {
        private readonly ICapPublisher _capPublisher;

        public PublisherTest()
        {
            _capPublisher = GetService<ICapPublisher>();

        }

        [Fact]
        public async Task Publish_Message_Test()
        {
            await _capPublisher.PublishAsync("sharpabp.abp.cap.test1", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        }

    }
}
