using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.FileStoring.S3
{
    public class DefaultS3FileNameCalculator : IS3FileNameCalculator, ITransientDependency
    {
        protected IFilePathBuilder FilePathBuilder { get; }

        public DefaultS3FileNameCalculator(IFilePathBuilder filePathBuilder)
        {
            FilePathBuilder = filePathBuilder;
        }

        public virtual string Calculate(FileProviderArgs args)
        {
            return FilePathBuilder.Build(args);
        }
    }
}
