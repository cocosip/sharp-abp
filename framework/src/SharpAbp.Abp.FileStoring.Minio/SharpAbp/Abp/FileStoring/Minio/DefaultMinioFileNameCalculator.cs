using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.FileStoring.Minio
{
    public class DefaultMinioFileNameCalculator : IMinioFileNameCalculator, ITransientDependency
    {
        protected IFilePathBuilder FilePathBuilder { get; }

        public DefaultMinioFileNameCalculator(IFilePathBuilder filePathBuilder)
        {
            FilePathBuilder = filePathBuilder;
        }

        public virtual string Calculate(FileProviderArgs args)
        {
            return FilePathBuilder.Build(args);
        }
    }
}
