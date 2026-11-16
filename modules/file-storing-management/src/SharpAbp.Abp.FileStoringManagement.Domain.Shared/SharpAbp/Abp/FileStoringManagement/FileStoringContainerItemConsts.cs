namespace SharpAbp.Abp.FileStoringManagement
{
    public static class FileStoringContainerItemConsts
    {
        /// <summary>
        /// Default value: 128.
        /// </summary>
        public static int MaxNameLength { get; set; } = 128;

        /// <summary>
        /// Default value: 2000.
        /// Kept at the Oracle NVARCHAR2 character limit so Oracle maps the column to NVARCHAR2(n) instead of a LOB.
        /// </summary>
        public static int MaxValueLength { get; set; } = 2000;

    }

}
