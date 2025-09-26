﻿﻿﻿﻿using System.Threading;

namespace SharpAbp.MinId
{
    /// <summary>
    /// Represents a segment of IDs that can be used for unique identifier generation.
    /// This class manages thread-safe ID allocation within a specific range and handles
    /// distributed ID generation with delta/remainder modulo operations.
    /// </summary>
    public class SegmentId
    {
        private readonly object _sync = new();
        private long _currentId;

        /// <summary>
        /// Gets or sets the current ID position within this segment.
        /// This value is incremented during ID generation and must be thread-safe.
        /// </summary>
        public long CurrentId
        {
            get { return _currentId; }
            set { _currentId = value; }
        }

        /// <summary>
        /// Gets or sets the maximum ID value that this segment can generate.
        /// Once CurrentId exceeds this value, the segment is exhausted.
        /// </summary>
        public long MaxId { get; set; }

        /// <summary>
        /// Gets or sets the delta value used for modulo operations in distributed scenarios.
        /// This ensures proper ID distribution across multiple instances.
        /// </summary>
        public int Delta { get; set; }

        /// <summary>
        /// Gets or sets the remainder value used with delta for modulo operations.
        /// This helps maintain uniqueness in distributed environments.
        /// </summary>
        public int Remainder { get; set; }

        /// <summary>
        /// Gets or sets the loading threshold ID.
        /// When CurrentId reaches this value, it signals that the next segment should be pre-loaded.
        /// </summary>
        public long LoadingId { get; set; }

        private bool _isSetup = false;

        /// <summary>
        /// Determines whether this segment still has available IDs for generation.
        /// </summary>
        /// <returns>True if the current ID is within the valid range; otherwise, false.</returns>
        public bool IsUseful()
        {
            return CurrentId <= MaxId;
        }

        /// <summary>
        /// Initializes the segment to ensure the current ID aligns with the delta/remainder requirements.
        /// This method is called once per segment and ensures thread-safe initialization.
        /// In distributed scenarios, this alignment ensures proper ID distribution across instances.
        /// </summary>
        public void Setup()
        {
            if (_isSetup)
            {
                return;
            }

            lock (_sync)
            {
                if (_isSetup)
                {
                    return;
                }

                if (CurrentId % Delta == Remainder)
                {
                    _isSetup = true;
                    return;
                }

                var id = CurrentId;

                // Align the current ID to match the delta/remainder constraint
                for (int i = 0; i <= Delta; i++)
                {
                    id = Interlocked.Increment(ref _currentId);
                    if (id % Delta == Remainder)
                    {
                        // Compensate for the extra increment to avoid wasting an ID
                        Interlocked.Add(ref _currentId, (0 - Delta));
                        _isSetup = true;
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Generates the next ID from this segment in a thread-safe manner.
        /// This method handles delta-based incrementation and returns status codes
        /// to indicate segment state (normal, loading threshold reached, or exhausted).
        /// </summary>
        /// <returns>
        /// A tuple containing:
        /// - Status code: Normal (0), Loading threshold reached (1), or Segment exhausted (2)
        /// - Generated ID: The next unique identifier
        /// </returns>
        public (int, long) NextId()
        {
            Setup();

            var id = Interlocked.Add(ref _currentId, Delta);

            if (id > MaxId)
            {
                return new(ResultCodeConsts.Over, id);
            }
            if (id >= LoadingId)
            {
                return new(ResultCodeConsts.Loading, id);
            }
            return new(ResultCodeConsts.Normal, id);
        }
    }
}
