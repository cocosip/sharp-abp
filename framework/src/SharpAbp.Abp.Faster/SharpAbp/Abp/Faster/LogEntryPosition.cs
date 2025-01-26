using System.Collections.Generic;

namespace SharpAbp.Abp.Faster
{
    public class LogEntryPosition : List<Position>, IComparer<Position>
    {
        public LogEntryPosition()
        {

        }

        public int Compare(Position x, Position y)
        {
            return x.Address.CompareTo(y.Address);
        }
    }
}
