using System.Collections.Generic;
using System.Linq;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.FileStoring.Fakes
{
    public class FakeProviders : ISingletonDependency
    {
        public FakeFileProvider1 Provider1 { get; }

        public FakeFileProvider2 Provider2 { get; }

        public FakeProviders(IEnumerable<IFileProvider> providers)
        {
            Provider1 = providers.OfType<FakeFileProvider1>().Single();
            Provider2 = providers.OfType<FakeFileProvider2>().Single();
        }
    }
}
