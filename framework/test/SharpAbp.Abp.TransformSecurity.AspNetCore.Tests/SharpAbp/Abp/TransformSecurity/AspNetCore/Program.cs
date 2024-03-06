using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using SharpAbp.Abp.TransformSecurity.AspNetCore;
using Volo.Abp.AspNetCore.TestBase;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    EnvironmentName = Environments.Staging
});

await builder.RunAbpModuleAsync<AbpTransformSecurityAspNetCoreTestModule>();

public partial class Program
{

}