using Microsoft.AspNetCore.Builder;
using TodoApp;
using Volo.Abp.AspNetCore.TestBase;

var builder = WebApplication.CreateBuilder();
builder.Environment.ContentRootPath = GetWebProjectContentRootPathHelper.Get("TodoApp.Web.csproj"); 
await builder.RunAbpModuleAsync<TodoAppWebTestModule>(applicationName: "TodoApp.Web");

public partial class Program
{
}
