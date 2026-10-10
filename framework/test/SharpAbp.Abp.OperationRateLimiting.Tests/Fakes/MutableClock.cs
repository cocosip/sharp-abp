using System;
using Volo.Abp.Timing;

namespace SharpAbp.Abp.OperationRateLimiting.Fakes
{
    public class MutableClock : IClock
    {
        public DateTime Now { get; set; } = new DateTime(2030, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public DateTimeKind Kind => DateTimeKind.Utc;
        public bool SupportsMultipleTimezone => true;
        public DateTime Normalize(DateTime value) => value.ToUniversalTime();
        public DateTime ConvertToUserTime(DateTime value) => value;
        public DateTimeOffset ConvertToUserTime(DateTimeOffset value) => value;
        public DateTime ConvertToUtc(DateTime value) => value.ToUniversalTime();
    }
}
