using System.Collections.Generic;
using System.Linq;

namespace SharpAbp.Abp.Faster
{
    public class LogEntryList<T> : List<LogEntry<T>> where T : class
    {
        public LogEntryPosition GetPosition()
        {
            var position = new LogEntryPosition();
            if (Count == 0)
            {
                return position;
            }

            var min = this.Min(x => x.CurrentAddress);
            var max = this.Max(x => x.CurrentAddress);

            var minEntry = this.Where(x => x.CurrentAddress == min).FirstOrDefault();
            var maxEntry = this.Where(x => x.CurrentAddress == max).FirstOrDefault();

            position.Min = new Position(minEntry.CurrentAddress, minEntry.EntryLength);
            position.Max = new Position(maxEntry.CurrentAddress, maxEntry.EntryLength);
            return position;
        }

    }
}
