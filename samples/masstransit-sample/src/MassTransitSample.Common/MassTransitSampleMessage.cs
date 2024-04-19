using MassTransit;
using System;

namespace MassTransitSample.Common
{
    [ExcludeFromTopology]
    public class MassTransitSampleMessage
    {
        /// <summary>
        /// 消息Id
        /// </summary>
        public string MessageId { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        public int Sequence { get; set; }

        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime PublishTime { get; set; }
    }
}
