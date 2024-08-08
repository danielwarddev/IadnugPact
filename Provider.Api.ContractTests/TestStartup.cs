using System.Reflection;
using Provider.Api;

namespace Provider.ContractTests;

public class TestStartup
{
    private Startup _proxyStartup;

    public TestStartup(IConfiguration configuration)
    {
        _proxyStartup = new Startup(configuration);
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // This line is required for the test assembly to be able to register the real endpoints
        services.AddControllers().AddApplicationPart(Assembly.GetAssembly(typeof(Startup))!);
        _proxyStartup.ConfigureServices(services);
    }
    
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseMiddleware<ProviderStateMiddleware>();
        _proxyStartup.Configure(app, env);
    }
}