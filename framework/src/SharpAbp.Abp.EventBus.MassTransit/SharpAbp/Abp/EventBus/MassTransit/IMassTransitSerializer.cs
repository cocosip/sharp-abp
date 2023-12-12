using System;

namespace SharpAbp.Abp.EventBus.MassTransit
{
    public interface IMassTransitSerializer
    {
        byte[] Serialize(object obj);
        string SerializeToString(object obj);
        object Deserialize(byte[] value, Type type);
        T Deserialize<T>(byte[] value);
        object Deserialize(string value, Type type);
    }
}
