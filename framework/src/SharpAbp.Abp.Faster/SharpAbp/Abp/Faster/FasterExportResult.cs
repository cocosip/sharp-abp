using System.Collections.Generic;

namespace SharpAbp.Abp.Faster
{
    /// <summary>
    /// Represents the result of a FASTER log export operation.
    /// </summary>
    public class FasterExportResult
    {
        /// <summary>
        /// Gets or sets the starting address of the export range.
        /// </summary>
        public long FromAddress { get; set; }

        /// <summary>
        /// Gets or sets the ending address of the export range.
        /// Use this value as the fromAddress for the next incremental export.
        /// </summary>
        public long ToAddress { get; set; }

        /// <summary>
        /// Gets or sets the total number of entries exported.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the list of exported file paths.
        /// Multiple files are created when the entry count exceeds entriesPerFile.
        /// </summary>
        public List<string> FilePaths { get; set; } = [];
    }
}
