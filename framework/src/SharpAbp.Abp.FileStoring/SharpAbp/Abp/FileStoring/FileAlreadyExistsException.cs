using System;
using System.Runtime.Serialization;
using Volo.Abp;

namespace SharpAbp.Abp.FileStoring
{
    public class FileAlreadyExistsException : AbpException
    {
        public FileAlreadyExistsException()
        {

        }

        public FileAlreadyExistsException(string message)
            : base(message)
        {

        }

        public FileAlreadyExistsException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        public FileAlreadyExistsException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {

        }
    }
}