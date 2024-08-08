using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Consumer;

public class Program
{
    public static async Task Main(string[] args)
    {
        await Host.CreateDefaultBuilder(args)
            .ConfigureServices((_, services) =>
            {
                services.AddHttpClient<IProductClient, ProductClient>(client =>
                {
                    client.BaseAddress = new Uri("http://example.com");
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                });
            })
            .Build()
            .RunAsync();
    }
}
