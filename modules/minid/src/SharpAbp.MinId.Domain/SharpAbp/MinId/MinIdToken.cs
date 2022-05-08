using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace SharpAbp.MinId
{
    public class MinIdToken : AuditedAggregateRoot<Guid>
    {
        /// <summary>
        /// Token
        /// </summary>
        public virtual string Token { get; set; }

        /// <summary>
        /// BizType
        /// </summary>
        public virtual string BizType { get; set; }

        /// <summary>
        /// Remark
        /// </summary>
        public virtual string Remark { get; set; }

        public MinIdToken()
        {

        }


        public MinIdToken(Guid id, string bizType, string token, string remark)
        {
            Id = id;
            BizType = bizType;
            Token = token;
            Remark = remark;
        }

        public void Update(string bizType, string token, string remark)
        {
            BizType = bizType;
            Token = token;
            Remark = remark;
        }
    }
}
