namespace SharpAbp.FileSystem.S3
{
    /// <summary>S3权限
    /// </summary>
    public enum S3Acl
    {
        /// <summary>私有的
        /// </summary>
        Private = 1,

        /// <summary>公有读
        /// </summary>
        PublicRead = 2,

        /// <summary>公有读和写
        /// </summary>
        PublicReadWrite = 4,

        /// <summary>授权读
        /// </summary>
        AuthenticatedRead = 8

    }
}
