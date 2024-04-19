using MassTransit;
using MassTransitSample.Common;
using System;
using System.Runtime.CompilerServices;

namespace MassTransitSample.Producer
{
    public static class CorrelationInitializer
    {
#pragma warning disable CA2255
        [ModuleInitializer]
#pragma warning restore CA2255
        public static void Initialize()
        {
            MessageCorrelation.UseCorrelationId<MassTransitSampleMessage>(x => Guid.Parse(x.MessageId));
        }
    }
}
