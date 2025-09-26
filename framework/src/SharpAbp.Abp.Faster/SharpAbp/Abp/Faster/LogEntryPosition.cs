using System.Collections.Generic;

namespace SharpAbp.Abp.Faster
{
    /// <summary>
    /// Represents a collection of log entry positions that can be compared based on their addresses.
    /// Inherits from List&lt;Position&gt; and implements IComparer&lt;Position&gt; for sorting capabilities.
    /// </summary>
    public class LogEntryPosition : List<Position>, IComparer<Position>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogEntryPosition"/> class.
        /// </summary>
        public LogEntryPosition()
        {

        }

        /// <summary>
        /// Compares two positions based on their addresses.
        /// </summary>
        /// <param name="x">The first position to compare.</param>
        /// <param name="y">The second position to compare.</param>
        /// <returns>
        /// A signed integer that indicates the relative values of x and y:
        /// Less than zero when x is less than y,
        /// Zero when x equals y,
        /// Greater than zero when x is greater than y.
        /// </returns>
        public int Compare(Position x, Position y)
        {
            return x.Address.CompareTo(y.Address);
        }
    }
}