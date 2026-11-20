using System.Collections.Generic;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.OpenApi;
using Moq;
using Swashbuckle.AspNetCore.SwaggerGen;
using Xunit;

namespace SharpAbp.Abp.Swashbuckle.Versioning;

public class SwaggerDefaultValuesTest
{
    private readonly SwaggerDefaultValues _filter = new();

    [Fact]
    public void Should_Remove_Unsupported_Response_Formats_And_Keep_Default_Response()
    {
        var description = new ApiDescription { ActionDescriptor = new ActionDescriptor() };
        var success = new ApiResponseType { StatusCode = 200 };
        success.ApiResponseFormats.Add(new ApiResponseFormat { MediaType = "application/json" });
        description.SupportedResponseTypes.Add(success);
        var fallback = new ApiResponseType { IsDefaultResponse = true };
        fallback.ApiResponseFormats.Add(new ApiResponseFormat { MediaType = "application/problem+json" });
        description.SupportedResponseTypes.Add(fallback);
        description.SupportedResponseTypes.Add(new ApiResponseType { StatusCode = 404 });
        var operation = new OpenApiOperation
        {
            Responses = new OpenApiResponses
            {
                ["200"] = new OpenApiResponse { Content = new Dictionary<string, OpenApiMediaType>
                    { ["application/json"] = new(), ["application/xml"] = new() } },
                ["default"] = new OpenApiResponse { Content = new Dictionary<string, OpenApiMediaType>
                    { ["application/problem+json"] = new() } }
            }
        };

        _filter.Apply(operation, Context(description));

        Assert.Equal(new[] { "application/json" }, operation.Responses["200"].Content.Keys);
        Assert.Equal(new[] { "application/problem+json" }, operation.Responses["default"].Content.Keys);
        Assert.False(operation.Responses.ContainsKey("404"));
    }

    [Fact]
    public void Should_Fill_Parameter_Metadata_Without_Overwriting_Explicit_Values()
    {
        var metadata = new Mock<ModelMetadata>(ModelMetadataIdentity.ForType(typeof(int)));
        metadata.SetupGet(x => x.Description).Returns("Number of results");
        var description = new ApiDescription { ActionDescriptor = new ActionDescriptor() };
        description.ParameterDescriptions.Add(new ApiParameterDescription
            { Name = "limit", DefaultValue = 25, IsRequired = true, ModelMetadata = metadata.Object });
        description.ParameterDescriptions.Add(new ApiParameterDescription
            { Name = "explicit", DefaultValue = 30 });
        description.ParameterDescriptions.Add(new ApiParameterDescription
            { Name = "fallback", DefaultValue = "value" });
        description.ParameterDescriptions.Add(new ApiParameterDescription { Name = "optional" });
        var limit = new OpenApiParameter { Name = "limit", Schema = new OpenApiSchema() };
        var explicitValue = new OpenApiParameter
            { Name = "explicit", Description = "Keep this", Required = true, Schema = new OpenApiSchema { Default = JsonValue.Create(10) } };
        var fallback = new OpenApiParameter { Name = "fallback", Schema = new OpenApiSchema() };
        var optional = new OpenApiParameter { Name = "optional", Schema = new OpenApiSchema() };
        var undocumented = new OpenApiParameter { Name = "other", Description = "Unchanged" };
        var operation = new OpenApiOperation
            { Parameters = new List<IOpenApiParameter> { limit, explicitValue, fallback, optional, undocumented } };

        _filter.Apply(operation, Context(description));

        Assert.Equal("Number of results", limit.Description);
        Assert.True(limit.Required);
        Assert.Equal(25, ((OpenApiSchema)limit.Schema).Default.GetValue<int>());
        Assert.Equal("Keep this", explicitValue.Description);
        Assert.True(explicitValue.Required);
        Assert.Equal(10, ((OpenApiSchema)explicitValue.Schema).Default.GetValue<int>());
        Assert.Equal("value", ((OpenApiSchema)fallback.Schema).Default.GetValue<string>());
        Assert.False(optional.Required);
        Assert.Null(((OpenApiSchema)optional.Schema).Default);
        Assert.Equal("Unchanged", undocumented.Description);
    }

    private static OperationFilterContext Context(ApiDescription description) => new(description,
        Mock.Of<ISchemaGenerator>(), new SchemaRepository(), new OpenApiDocument(),
        typeof(SwaggerDefaultValuesTest).GetMethod(nameof(Should_Fill_Parameter_Metadata_Without_Overwriting_Explicit_Values)));
}
