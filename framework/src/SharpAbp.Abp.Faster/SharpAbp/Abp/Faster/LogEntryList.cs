﻿using System.Collections.Generic;

namespace SharpAbp.Abp.Faster
{
    /// <summary>
    /// Represents a collection of log entries of type T, inheriting from List&lt;LogEntry&lt;T&gt;&gt;.
    /// Provides functionality to extract position information from the log entries.
    /// </summary>
    /// <typeparam name="T">The type of data stored in the log entries. Must be a class.</typeparam>
    public class LogEntryList<T> : List<LogEntry<T>> where T : class
    {
        /// <summary>
        /// Extracts position information from all log entries in the collection.
        /// </summary>
        /// <returns>
        /// A list of Position objects containing address range information for all entries.
        /// Returns an empty list if the collection is empty.
        /// </returns>
        public List<Position> GetPositions()
        {
            var positions = new List<Position>(Count);

            foreach (var entry in this)
            {
                // Use the simplified Position constructor with just address and nextAddress
                positions.Add(new Position(entry.CurrentAddress, entry.NextAddress));
            }

            return positions;
        }
    }
}