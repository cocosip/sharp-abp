using JetBrains.Annotations;
using System;
using Volo.Abp;

namespace SharpAbp.Abp.FileStoring
{
    public static class FileStoringUtil
    {
        public static object ConvertPrimitiveType(string value, [NotNull] Type type, [NotNull] bool throwIfNotPrimitive = false)
        {
            Check.NotNull(type, nameof(type));

            if (type == typeof(string))
            {
                return value;
            }

            if (type.IsPrimitive)
            {
                if (value.IsNullOrWhiteSpace())
                {
                    return null;
                }

                return Convert.ChangeType(value, type);
            }

            if (throwIfNotPrimitive)
            {
                throw new AbpException($"Can't convert string to type '{type.FullName}',it's not a primitive type");
            }
            return null;
        }

        public static object ConvertPrimitiveType(string value, [NotNull] string typeName, [NotNull] bool throwIfNotPrimitive = false)
        {
            var type = Type.GetType(typeName);
            return ConvertPrimitiveType(value, type, throwIfNotPrimitive);
        }


        public static T ConvertPrimitiveType<T>(string value, bool throwIfNotPrimitive = false)
        {
            return (T)ConvertPrimitiveType(value, typeof(T), throwIfNotPrimitive);
        }
    }
}
