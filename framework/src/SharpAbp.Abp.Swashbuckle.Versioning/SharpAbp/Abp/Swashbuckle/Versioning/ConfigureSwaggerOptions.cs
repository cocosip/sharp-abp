using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SharpAbp.Abp.Swashbuckle.Versioning
{
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly AbpSwashbuckleVersioningOptions _swashbuckleVersioningOptions;
        private readonly IApiVersionDescriptionProvider _descriptionProvider;
        public ConfigureSwaggerOptions(
            IOptions<AbpSwashbuckleVersioningOptions> options,
            IApiVersionDescriptionProvider descriptionProvider)
        {
            _swashbuckleVersioningOptions = options.Value;
            _descriptionProvider = descriptionProvider;
        }

        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in _descriptionProvider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
            }
        }

        private OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var info = new OpenApiInfo()
            {
                Title = _swashbuckleVersioningOptions.Title,
                Version = description.ApiVersion.ToString(),
            };

            if (description.IsDeprecated)
            {
                info.Description += " This API version has been deprecated.";
            }

            return info;
        }
    }
}
