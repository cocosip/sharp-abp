using Polly;
using System;
using System.IO;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.IO;

namespace SharpAbp.Abp.FileStoring
{
    public class FileSystemFileProvider : FileProviderBase, ITransientDependency
    {
        protected IFilePathCalculator FilePathCalculator { get; }

        public FileSystemFileProvider(IFilePathCalculator filePathCalculator)
        {
            FilePathCalculator = filePathCalculator;
        }

        public override async Task SaveAsync(FileProviderSaveArgs args)
        {
            var filePath = FilePathCalculator.Calculate(args);

            if (!args.OverrideExisting && await ExistsAsync(filePath))
            {
                throw new FileAlreadyExistsException($"Saving BLOB '{args.FileName}' does already exists in the container '{args.ContainerName}'! Set {nameof(args.OverrideExisting)} if it should be overwritten.");
            }

            DirectoryHelper.CreateIfNotExists(Path.GetDirectoryName(filePath));

            var fileMode = args.OverrideExisting
                ? FileMode.Create
                : FileMode.CreateNew;

            await Policy.Handle<IOException>()
                .WaitAndRetryAsync(2, retryCount => TimeSpan.FromSeconds(retryCount))
                .ExecuteAsync(async () =>
                {
                    using (var fileStream = File.Open(filePath, fileMode, FileAccess.Write))
                    {
                        await args.FileStream.CopyToAsync(
                            fileStream,
                            args.CancellationToken
                        );

                        await fileStream.FlushAsync();
                    }
                });
        }

        public override Task<bool> DeleteAsync(FileProviderDeleteArgs args)
        {
            var filePath = FilePathCalculator.Calculate(args);
            return Task.FromResult(FileHelper.DeleteIfExists(filePath));
        }

        public override Task<bool> ExistsAsync(FileProviderExistsArgs args)
        {
            var filePath = FilePathCalculator.Calculate(args);
            return ExistsAsync(filePath);
        }

        public override async Task<Stream> GetOrNullAsync(FileProviderGetArgs args)
        {
            var filePath = FilePathCalculator.Calculate(args);

            if (!File.Exists(filePath))
            {
                return null;
            }

            return await Policy.Handle<IOException>()
                .WaitAndRetryAsync(2, retryCount => TimeSpan.FromSeconds(retryCount))
                .ExecuteAsync(async () =>
                {
                    using (var fileStream = File.OpenRead(filePath))
                    {
                        var memoryStream = new MemoryStream();
                        await fileStream.CopyToAsync(memoryStream, args.CancellationToken);
                        return memoryStream;
                    }
                });
        }

        protected virtual Task<bool> ExistsAsync(string filePath)
        {
            return Task.FromResult(File.Exists(filePath));
        }
    }
}
