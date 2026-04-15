#nullable enable
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.MultiTenancy;
using Xunit;

namespace SharpAbp.Abp.AspNetCore.MapTenancy
{
    public class MapFormTenantResolveContributorTest
    {
        [Fact]
        public async Task ResolveAsync_Should_Return_Null_For_Invalid_Form_Data()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.ContentType = "application/x-www-form-urlencoded";
            httpContext.Features.Set<IFormFeature>(new ThrowingFormFeature());

            var resolveContext = new Mock<ITenantResolveContext>();
            resolveContext.SetupGet(x => x.ServiceProvider).Returns(new ServiceCollection().BuildServiceProvider());

            var contributor = new TestableMapFormTenantResolveContributor();
            var result = await contributor.ResolveAsync(resolveContext.Object, httpContext);

            Assert.Null(result);
        }

        private sealed class TestableMapFormTenantResolveContributor : MapFormTenantResolveContributor
        {
            public Task<string?> ResolveAsync(ITenantResolveContext context, HttpContext httpContext)
            {
                return GetTenantIdOrNameFromHttpContextOrNullAsync(context, httpContext);
            }
        }

        private sealed class ThrowingFormFeature : IFormFeature
        {
            public bool HasFormContentType => true;

            public IFormCollection? Form
            {
                get => null;
                set { }
            }

            public IFormCollection ReadForm()
            {
                throw new InvalidDataException("bad form");
            }

            public Task<IFormCollection> ReadFormAsync(CancellationToken cancellationToken)
            {
                throw new InvalidDataException("bad form");
            }
        }
    }
}
