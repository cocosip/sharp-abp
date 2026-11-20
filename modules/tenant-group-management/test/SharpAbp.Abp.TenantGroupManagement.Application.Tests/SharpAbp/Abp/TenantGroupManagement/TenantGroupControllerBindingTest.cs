using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace SharpAbp.Abp.TenantGroupManagement;

public class TenantGroupControllerBindingTest
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Connection_String_Update_Should_Bind_Only_The_Json_Body(bool includeBody)
    {
        const string bodyValue = "Server=example;Password=body-only";
        const string queryValue = "Server=example;Password=query-must-not-bind";
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddControllers().AddApplicationPart(typeof(TenantGroupController).Assembly);
        using var provider = services.BuildServiceProvider();
        var action = provider.GetRequiredService<IActionDescriptorCollectionProvider>().ActionDescriptors.Items
            .OfType<ControllerActionDescriptor>()
            .Single(item => item.ControllerTypeInfo.AsType() == typeof(TenantGroupController) &&
                item.MethodInfo.Name == nameof(TenantGroupController.UpdateDefaultConnectionStringAsync));
        var parameter = action.Parameters.Single(item => item.Name == "defaultConnectionString");
        var context = new DefaultHttpContext { RequestServices = provider };
        context.Request.Method = "PUT";
        context.Request.QueryString = QueryString.Create("defaultConnectionString", queryValue);
        context.Request.ContentType = "application/json";
        var bytes = includeBody ? Encoding.UTF8.GetBytes(JsonSerializer.Serialize(bodyValue)) : System.Array.Empty<byte>();
        using var body = new MemoryStream(bytes);
        context.Request.Body = body;
        context.Request.ContentLength = bytes.Length;
        var actionContext = new ActionContext(context, new RouteData(), action, new ModelStateDictionary());
        var metadata = provider.GetRequiredService<IModelMetadataProvider>().GetMetadataForType(typeof(string));
        var binder = provider.GetRequiredService<IModelBinderFactory>().CreateBinder(new ModelBinderFactoryContext
        {
            Metadata = metadata,
            BindingInfo = parameter.BindingInfo,
            CacheToken = parameter
        });
        var binding = DefaultModelBindingContext.CreateBindingContext(actionContext,
            new QueryStringValueProvider(BindingSource.Query, context.Request.Query, CultureInfo.InvariantCulture),
            metadata, parameter.BindingInfo, parameter.Name);

        await binder.BindModelAsync(binding);

        if (includeBody)
        {
            Assert.True(binding.Result.IsModelSet);
            Assert.Equal(bodyValue, binding.Result.Model);
            Assert.True(actionContext.ModelState.IsValid);
        }
        else
        {
            Assert.False(binding.Result.IsModelSet);
            Assert.False(actionContext.ModelState.IsValid);
        }
    }
}
