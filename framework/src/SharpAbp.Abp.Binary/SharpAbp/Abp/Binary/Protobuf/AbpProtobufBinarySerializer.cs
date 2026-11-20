using Microsoft.Extensions.Options;
using ProtoBuf;
using System;
using System.IO;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Binary.Protobuf
{

    public class AbpProtobufBinarySerializer : IBinarySerializer, ITransientDependency
    {
        protected AbpProtobufSerializerOptions Options { get; }
        public AbpProtobufBinarySerializer(IOptions<AbpProtobufSerializerOptions> options)
        {
            Options = options.Value;
        }

        public virtual byte[] Serialize(object obj)
        {
            using var ms = new MemoryStream();
            Serializer.Serialize(ms, obj);
            return ms.ToArray();
        }

        public virtual T Deserialize<T>(byte[] buffer)
        {
            return Serializer.Deserialize<T>(buffer.AsMemory());
        }

        public virtual object Deserialize(byte[] buffer, Type type)
        {
            return Serializer.NonGeneric.Deserialize(type, buffer.AsMemory());
        }

    }
}
