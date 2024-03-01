using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Modularity;
using Volo.Abp.Swashbuckle;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.Swashbuckle.Versioning
{
    [DependsOn(
        typeof(AbpSwashbuckleModule)
        )]
    public class AbpSwashbuckleVersioningModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
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
            return Task.CompletedTask;
        }



        protected virtual void AddApiVersioning(ServiceConfigurationContext context)
        {
            var apiVersioningConfigures = context.Services.GetPreConfigureActions<ApiVersioningOptions>();

            context.Services.AddAbpApiVersioning(options =>
            {
                // Show neutral/versionless APIs.
                //options.UseApiBehavior = false;
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

            context.Services.AddApiVersioning(options => { })
                .AddApiExplorer(options =>
                {
                    options.GroupNameFormat = "'v'VVV";
                    options.SubstituteApiVersionInUrl = true;
                    if (apiExplorerConfigures != null)
                    {
                        foreach (var action in apiExplorerConfigures)
                        {
                            action(options);
                        }
                    }
                });
        }

    }
}
