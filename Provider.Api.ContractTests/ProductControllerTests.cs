using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;
using PactNet;
using PactNet.Infrastructure.Outputters;
using PactNet.Output.Xunit;
using PactNet.Verifier;
using Provider.Api;
using Xunit.Abstractions;

namespace Provider.ContractTests;

public class ProductApiFixture : IDisposable
{
    private readonly IHost _server;
    private IProductService ProductServiceMock { get; } = Substitute.For<IProductService>();
    public Uri PactServerUri { get; } = new ("http://localhost:59168");
 
    public ProductApiFixture()
    {
        _server = Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(builder =>
            {
                builder.UseUrls(PactServerUri.ToString());
                builder.UseStartup<TestStartup>();
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<IProductService>();
                    services.AddSingleton<IProductService>(_ => ProductServiceMock);
                });
            })
            .Build();
        
        _server.Start();
    }

    public void Dispose()
    {
        _server.Dispose();
    }
}

public class ProductControllerTests : IClassFixture<ProductApiFixture>
{
    private readonly ProductApiFixture _apiFixture;
    private readonly ITestOutputHelper _outputHelper;
    
    public ProductControllerTests(ProductApiFixture apiFixture, ITestOutputHelper outputHelper)
    {
        _apiFixture = apiFixture;
        _outputHelper = outputHelper;
    }
    
    [Fact]
    public void Verify_MyService_Pact_Is_Honored()
    {
        var config = new PactVerifierConfig
        {
            LogLevel = PactLogLevel.Debug,
            Outputters = new List<IOutput> { new XunitOutput(_outputHelper) }
        };

        var pactPath = Path.Combine("Pacts", "My Consumer Service-Product API.json");
        using var pactVerifier = new PactVerifier("Product API", config);
        
        pactVerifier
            .WithHttpEndpoint(_apiFixture.PactServerUri)
            .WithFileSource(new FileInfo(pactPath))
            .WithProviderStateUrl(new Uri(_apiFixture.PactServerUri, "/provider-states"))
            .Verify();
    }
}
