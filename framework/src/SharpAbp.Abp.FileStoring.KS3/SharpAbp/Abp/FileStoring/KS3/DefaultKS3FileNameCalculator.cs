using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.FileStoring.KS3
{
    public class DefaultKS3FileNameCalculator : IKS3FileNameCalculator, ITransientDependency
    {
        protected IFilePathBuilder FilePathBuilder { get; }

        public DefaultKS3FileNameCalculator(IFilePathBuilder filePathBuilder)
        {
            FilePathBuilder = filePathBuilder;
        }

        public virtual string Calculate(FileProviderArgs args)
        {
            return FilePathBuilder.Build(args);
        }
    }
}
