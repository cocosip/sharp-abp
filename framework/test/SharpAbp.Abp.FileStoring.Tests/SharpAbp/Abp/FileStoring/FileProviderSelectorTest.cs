using SharpAbp.Abp.FileStoring.Fakes;
using SharpAbp.Abp.FileStoring.TestObjects;
using System;
using Xunit;

namespace SharpAbp.Abp.FileStoring
{
    public class FileProviderSelectorTest : AbpFileStoringTestBase
    {

        private readonly IFileProviderSelector _selector;
        public FileProviderSelectorTest()
        {
            _selector = GetRequiredService<IFileProviderSelector>();
        }

        [Fact]
        public void Should_Select_Default_Provider_If_Not_Configured()
        {
            var container = _selector.Get<TestContainer3>();
            Assert.True(container.GetType().IsAssignableTo(typeof(FakeFileProvider1)));
        }

        [Fact]
        public void Should_Select_Configured_Provider()
        {
            var container1 = _selector.Get<TestContainer1>();
            var container2 = _selector.Get<TestContainer2>();
            Assert.True(container1.GetType().IsAssignableTo(typeof(FakeFileProvider1)));
            Assert.True(container2.GetType().IsAssignableTo(typeof(FakeFileProvider2)));
        }


    }
}
