using MassTransit.RabbitMqTransport;
using System;

namespace SharpAbp.Abp.MassTransit.RabbitMQ
{
    public class AbpMassTransitRabbitMqOptions
    {
        public string Host { get; set; }
        public ushort Port { get; set; }
        public string VirtualHost { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool UseSsl { get; set; }
        public Action<IRabbitMqSslConfigurator> ConfigureSsl { get; set; }



    }
}
