using System.Collections.Generic;

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

            foreach (var entry in this)
            {
                position.Add(new Position(entry.CurrentAddress, entry.EntryLength, entry.NextAddress));
            }

            return position;
        }

    }
}
