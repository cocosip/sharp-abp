﻿using System;

namespace SharpAbp.Abp.FileStoring.Aws
{
    [Serializable]
    public class AwsTemporaryCredentialsCacheItem
    {
        public string AccessKeyId { get; set; }

        public string SecretAccessKey { get; set; }

        public string SessionToken { get; set; }

 
        public AwsTemporaryCredentialsCacheItem(string accessKeyId, string secretAccessKey, string sessionToken)
        {
            AccessKeyId = accessKeyId;
            SecretAccessKey = secretAccessKey;
            SessionToken = sessionToken;
        }
    }
}
