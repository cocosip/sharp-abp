using System;

namespace SharpAbp.Abp.FileStoring.Aliyun
{
    [Serializable]
    public class AliyunTemporaryCredentialsCacheItem
    {
        public string AccessKeyId { get; set; }

        public string AccessKeySecret { get; set; }

        public string SecurityToken { get; set; }

        public AliyunTemporaryCredentialsCacheItem()
        {

        }

        public AliyunTemporaryCredentialsCacheItem(string accessKeyId, string accessKeySecret, string securityToken)
        {
            AccessKeyId = accessKeyId;
            AccessKeySecret = accessKeySecret;
            SecurityToken = securityToken;
        }
    }
}
