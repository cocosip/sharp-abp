using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace SharpAbp.MinId
{
    public class MinIdInfo : AuditedAggregateRoot<Guid>
    {
        /// <summary>
        /// BizType
        /// </summary>
        public virtual string BizType { get; set; }

        /// <summary>
        /// MaxId
        /// </summary>
        public virtual long MaxId { get; set; }

        /// <summary>
        /// Step
        /// </summary>
        public virtual int Step { get; set; }

        /// <summary>
        /// Delta
        /// </summary>
        public virtual int Delta { get; set; }

        /// <summary>
        /// Remainder
        /// </summary>
        public virtual int Remainder { get; set; }

        public MinIdInfo()
        {

        }

        public MinIdInfo(Guid id, string bizType, long maxId, int step, int delta, int remainder)
        {
            Id = id;
            BizType = bizType;
            MaxId = maxId;
            Step = step;
            Delta = delta;
            Remainder = remainder;
        }

        public void Update(string bizType, long maxId, int step, int delta, int remainder)
        {
            BizType = bizType;
            MaxId = maxId;
            Step = step;
            Delta = delta;
            Remainder = remainder;
        }

        public void UpdateMaxId(long maxId)
        {
            MaxId = maxId;
        }

    }
}
