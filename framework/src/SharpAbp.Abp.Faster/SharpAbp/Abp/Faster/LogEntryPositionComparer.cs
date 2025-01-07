using System.Collections.Generic;

namespace SharpAbp.Abp.Faster
{
    public class LogEntryPositionComparer : IComparer<LogEntryPosition>
    {
        public int Compare(LogEntryPosition x, LogEntryPosition y)
        {
            if (x == null || y == null)
            {
                return 0;
            }

            if (x == null)
            {
                return -1;
            }

            if (y == null)
            {
                return 1;
            }

            if (x.Min.CompareTo(y.Min) == 0)
            {
                return x.Max.CompareTo(y.Max);
            }

            return x.Min.CompareTo(y.Min);
        }
    }
}
