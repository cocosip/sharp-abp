namespace SharpAbp.Abp.FileStoring
{
    public static class FileContainerFactoryExtensions
    {
        /// <summary>
        /// Gets a named container.
        /// </summary>
        /// <param name="fileContainerFactory">The file container manager</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// The container object.
        /// </returns>
        public static IFileContainer Create<TContainer>(
            this IFileContainerFactory fileContainerFactory
        )
        {
            return fileContainerFactory.Create(
                FileContainerNameAttribute.GetContainerName<TContainer>()
            );
        }
    }
}