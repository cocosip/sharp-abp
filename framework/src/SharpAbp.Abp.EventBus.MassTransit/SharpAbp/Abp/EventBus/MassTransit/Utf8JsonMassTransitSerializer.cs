using System;
using System.Text;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Json;

namespace SharpAbp.Abp.EventBus.MassTransit
{
    public class Utf8JsonMassTransitSerializer : IMassTransitSerializer, ITransientDependency
    {
        private readonly IJsonSerializer _jsonSerializer;

        public Utf8JsonMassTransitSerializer(IJsonSerializer jsonSerializer)
        {
            _jsonSerializer = jsonSerializer;
        }

        public byte[] Serialize(object obj)
        {
            return Encoding.UTF8.GetBytes(_jsonSerializer.Serialize(obj));
        }

        public string SerializeToString(object obj)
        {
            return _jsonSerializer.Serialize(obj);
        }


        public object Deserialize(byte[] value, Type type)
        {
            return _jsonSerializer.Deserialize(type, Encoding.UTF8.GetString(value));
        }

        public T Deserialize<T>(byte[] value)
        {
            return _jsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(value));
        }


        public object Deserialize(string value, Type type)
        {
            return _jsonSerializer.Deserialize(type, value);
        }
    }
}
