namespace Provider.Api;

public class Startup
{
    public IConfiguration Configuration { get; }
    
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }
    
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddScoped<IProductService, ProductService>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.UseRouting().UseEndpoints(config => config.MapControllers());
    }
}