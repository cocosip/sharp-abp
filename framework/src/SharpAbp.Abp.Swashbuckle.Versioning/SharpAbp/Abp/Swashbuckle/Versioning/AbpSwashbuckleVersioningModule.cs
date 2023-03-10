using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Modularity;
using Volo.Abp.Swashbuckle;

namespace SharpAbp.Abp.Swashbuckle.Versioning
{
    [DependsOn(
        typeof(AbpSwashbuckleModule)
        )]
    public class AbpSwashbuckleVersioningModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpSwashbuckleVersioningOptions>(options =>
            {
                options.Title = "SharpAbp";
            });
            context.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            //ApiVersioning
            AddApiVersioning(context);

            //ApiExplorer
            AddVersionedApiExplorer(context);
 
            Configure<AbpAspNetCoreMvcOptions>(options =>
            {
                options.ChangeControllerModelApiExplorerGroupName = false;
            });
        }


        protected virtual void AddApiVersioning(ServiceConfigurationContext context)
        {
            var apiVersioningConfigures = context.Services.GetPreConfigureActions<ApiVersioningOptions>();

            context.Services.AddAbpApiVersioning(options =>
            {
                // Show neutral/versionless APIs.
                options.UseApiBehavior = false;
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);

                if (apiVersioningConfigures != null)
                {
                    foreach (var action in apiVersioningConfigures)
                    {
                        action(options);
                    }
                }
            });
        }


        protected virtual void AddVersionedApiExplorer(ServiceConfigurationContext context)
        {
            var apiExplorerConfigures = context.Services.GetPreConfigureActions<ApiExplorerOptions>();
            context.Services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;

                if (apiExplorerConfigures != null)
                {
                    foreach(var action in apiExplorerConfigures)
                    {
                        action(options);
                    }
                }
            });
        }

    }
}
