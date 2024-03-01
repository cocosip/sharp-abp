namespace SharpAbp.Abp.TransformSecurity.AspNetCore
{
    public class SecurityPublicKeyDto  
    {
        /// <summary>
        /// 唯一编号
        /// </summary>
        public string UniqueId { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        public string BizType { get; set; }

        /// <summary>
        /// 密钥类型, RSA, SM2
        /// </summary>
        public string KeyType { get; set; }

        /// <summary>
        /// 公钥
        /// </summary>
        public string PublicKey { get; set; }

    }
}
