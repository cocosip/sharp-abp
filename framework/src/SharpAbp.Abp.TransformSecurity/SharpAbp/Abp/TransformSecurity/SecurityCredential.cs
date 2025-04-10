﻿using System;
using Volo.Abp.ObjectExtending;

namespace SharpAbp.Abp.TransformSecurity
{
    /// <summary>
    /// 安全密钥对
    /// </summary>
    [Serializable]
    public class SecurityCredential : ExtensibleObject
    {
        /// <summary>
        /// 唯一编号
        /// </summary>
        public string? Identifier { get; set; }

        /// <summary>
        /// 密钥类型, RSA, SM2
        /// </summary>
        public string? KeyType { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        public string? BizType { get; set; }

        /// <summary>
        /// 公钥
        /// </summary>
        public string? PublicKey { get; set; }

        /// <summary>
        /// 私钥
        /// </summary>
        public string? PrivateKey { get; set; }

        /// <summary>
        /// 密钥的过期时间
        /// </summary>
        public DateTime? Expires { get; set; }

        /// <summary>
        /// 密钥的创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }


        public bool IsExpires(DateTime dateTime)
        {
            return Expires.HasValue && Expires.Value <= dateTime;
        }
    }
}
