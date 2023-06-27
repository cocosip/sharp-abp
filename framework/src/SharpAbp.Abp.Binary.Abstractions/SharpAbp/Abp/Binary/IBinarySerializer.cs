using System;

namespace SharpAbp.Abp.Binary
{
    public interface IBinarySerializer
    {
        byte[] Serialize(object obj);
        T Deserialize<T>(byte[] buffer);
        object Deserialize(byte[] buffer, Type type);
    }
}
