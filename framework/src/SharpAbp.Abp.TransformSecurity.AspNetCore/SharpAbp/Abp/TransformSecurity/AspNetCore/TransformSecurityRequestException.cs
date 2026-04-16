using Volo.Abp;

namespace SharpAbp.Abp.TransformSecurity.AspNetCore
{
    public class TransformSecurityRequestException : AbpException
    {
        public int StatusCode { get; }

        public TransformSecurityRequestException(int statusCode, string message)
            : base(message)
        {
            StatusCode = statusCode;
        }

        public TransformSecurityRequestException(int statusCode, string message, System.Exception innerException)
            : base(message, innerException)
        {
            StatusCode = statusCode;
        }
    }
}
