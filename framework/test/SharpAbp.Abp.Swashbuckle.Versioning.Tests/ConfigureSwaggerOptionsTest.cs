using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Moq;
using Swashbuckle.AspNetCore.SwaggerGen;
using Xunit;

namespace SharpAbp.Abp.Swashbuckle.Versioning;

public class ConfigureSwaggerOptionsTest
{
    [Fact]
    public void Should_Create_One_Document_Per_Version_And_Mark_Only_Deprecated_Versions()
    {
        var descriptions = new Mock<IApiVersionDescriptionProvider>();
        descriptions.SetupGet(x => x.ApiVersionDescriptions).Returns(new[]
        {
            new ApiVersionDescription(new ApiVersion(1, 0), "v1", true),
            new ApiVersionDescription(new ApiVersion(2, 0), "v2", false)
        });
        var configure = new ConfigureSwaggerOptions(Options.Create(new AbpSwashbuckleVersioningOptions { Title = "Example API" }), descriptions.Object);
        var options = new SwaggerGenOptions();

        configure.Configure(options);

        var documents = options.SwaggerGeneratorOptions.SwaggerDocs;
        Assert.Equal(2, documents.Count);
        Assert.Equal("Example API", documents["v1"].Title);
        Assert.Equal("1.0", documents["v1"].Version);
        Assert.Contains("deprecated", documents["v1"].Description);
        Assert.Equal("2.0", documents["v2"].Version);
        Assert.Null(documents["v2"].Description);
    }
}
