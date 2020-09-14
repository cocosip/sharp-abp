namespace SharpAbp.Abp.FileStoring
{
    public interface IFileContainerFactory
    {
        /// <summary>
        /// Gets a named container.
        /// </summary>
        /// <param name="name">The name of the container</param>
        /// <returns>
        /// The container object.
        /// </returns>
        IFileContainer Create(
            string name
        );
    }
}