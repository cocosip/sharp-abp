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
        /// A LogEntryPosition object containing position information for all entries.
        /// Returns an empty LogEntryPosition if the collection is empty.
        /// </returns>
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