using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using SharpAbp.Abp.AspNetCore;
using Volo.Abp.AspNetCore.TestBase;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    EnvironmentName = Environments.Staging
});

await builder.RunAbpModuleAsync<SharpAbpAspNetCoreTestModule>();

public partial class Program
{

}