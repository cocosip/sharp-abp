using System;
using Volo.Abp.Domain.Entities.Auditing;
namespace SharpAbp.Abp.CryptoVault
{
    public class RSACreds : AuditedAggregateRoot<Guid>
    {
        /// <summary>
        /// 唯一标志
        /// </summary>
        public virtual string Identity { get; set; }


        public RSACreds()
        {
          
        }


    }
}
