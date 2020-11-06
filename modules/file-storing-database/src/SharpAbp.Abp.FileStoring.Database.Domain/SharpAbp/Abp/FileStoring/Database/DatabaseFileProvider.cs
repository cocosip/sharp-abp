using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;

namespace SharpAbp.Abp.FileStoring.Database
{
    public class DatabaseFileProvider : FileProviderBase, ITransientDependency
    {
        protected IDatabaseFileRepository DatabaseFileRepository { get; }
        protected IDatabaseFileContainerRepository DatabaseFileContainerRepository { get; }
        protected IGuidGenerator GuidGenerator { get; }
        protected ICurrentTenant CurrentTenant { get; }

        public DatabaseFileProvider(
           IDatabaseFileRepository databaseFileRepository,
           IDatabaseFileContainerRepository databaseFileContainerRepository,
           IGuidGenerator guidGenerator,
           ICurrentTenant currentTenant)
        {
            DatabaseFileRepository = databaseFileRepository;
            DatabaseFileContainerRepository = databaseFileContainerRepository;
            GuidGenerator = guidGenerator;
            CurrentTenant = currentTenant;
        }

        public override string Provider => DatabaseFileProviderConsts.ProviderName;

        public override async Task<string> SaveAsync(FileProviderSaveArgs args)
        {
            var container = await GetOrCreateContainerAsync(args.ContainerName, args.CancellationToken);

            var file = await DatabaseFileRepository.FindAsync(
                container.Id,
                args.ContainerName,
                args.CancellationToken
            );

            var content = await args.FileStream.GetAllBytesAsync(args.CancellationToken);

            if (file != null)
            {
                if (!args.OverrideExisting)
                {
                    throw new FileAlreadyExistsException(
                        $"Saving File '{args.FileId}' does already exists in the container '{args.ContainerName}'! Set {nameof(args.OverrideExisting)} if it should be overwritten.");
                }

                file.SetContent(content);

                await DatabaseFileRepository.UpdateAsync(file, autoSave: true);
            }
            else
            {
                file = new DatabaseFile(GuidGenerator.Create(), container.Id, args.FileId, content, CurrentTenant.Id);
                await DatabaseFileRepository.InsertAsync(file, autoSave: true);
            }

            return args.FileId;
        }

        public override async Task<bool> DeleteAsync(FileProviderDeleteArgs args)
        {
            var container = await DatabaseFileContainerRepository.FindAsync(
                args.ContainerName,
                args.CancellationToken
            );

            if (container == null)
            {
                return false;
            }

            return await DatabaseFileRepository.DeleteAsync(
                container.Id,
                args.FileId,
                autoSave: true,
                cancellationToken: args.CancellationToken
            );
        }

        public override async Task<bool> ExistsAsync(FileProviderExistsArgs args)
        {
            var container = await DatabaseFileContainerRepository.FindAsync(
                args.ContainerName,
                args.CancellationToken
            );

            if (container == null)
            {
                return false;
            }

            return await DatabaseFileRepository.ExistsAsync(
                container.Id,
                args.FileId,
                args.CancellationToken
            );
        }

        public override async Task<Stream> GetOrNullAsync(FileProviderGetArgs args)
        {
            var container = await DatabaseFileContainerRepository.FindAsync(
                args.ContainerName,
                args.CancellationToken
            );

            if (container == null)
            {
                return null;
            }

            var file = await DatabaseFileRepository.FindAsync(
                container.Id,
                args.FileId,
                args.CancellationToken
            );

            if (file == null)
            {
                return null;
            }

            return new MemoryStream(file.Content);
        }

        public override async Task<bool> DownloadAsync(FileProviderDownloadArgs args)
        {
            var container = await DatabaseFileContainerRepository.FindAsync(
                args.ContainerName,
                args.CancellationToken
            );

            if (container == null)
            {
                return false;
            }

            var file = await DatabaseFileRepository.FindAsync(
                container.Id,
                args.FileId,
                args.CancellationToken
            );

            if (file == null)
            {
                return false;
            }

            using (var fs = new FileStream(args.Path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                await fs.WriteAsync(file.Content, 0, file.Content.Length);
            }
            return true;
        }


        public override async Task<string> GetAccessUrlAsync(FileProviderAccessArgs args)
        {
            var container = await DatabaseFileContainerRepository.FindAsync(
              args.ContainerName,
              args.CancellationToken
            );

            if (container == null)
            {
                return string.Empty;
            }

            if (!container.HttpSupport)
            {
                return string.Empty;
            }

            var file = await DatabaseFileRepository.FindAsync(
             container.Id,
             args.FileId,
             args.CancellationToken);

            if (file == null)
            {
                return string.Empty;
            }

            return BuildAccessUrl(container, file.Name);
        }


        protected virtual async Task<DatabaseFileContainer> GetOrCreateContainerAsync(
           string name,
           CancellationToken cancellationToken = default)
        {
            var container = await DatabaseFileContainerRepository.FindAsync(name, cancellationToken);
            if (container != null)
            {
                return container;
            }

            container = new DatabaseFileContainer(GuidGenerator.Create(), name, CurrentTenant.Id);
            await DatabaseFileContainerRepository.InsertAsync(container, cancellationToken: cancellationToken);
            return container;
        }


        protected virtual string BuildAccessUrl(DatabaseFileContainer container, string fileId)
        {
            var accessUrl = container.IncludeContainer ? $"{container.HttpServer.TrimEnd('/')}/{container.Name}/{fileId}" : $"{container.HttpServer.TrimEnd('/')}/{fileId}";

            return accessUrl;
        }


    }
}
