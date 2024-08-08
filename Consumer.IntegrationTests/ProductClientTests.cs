using System.Net;
using FluentAssertions;
using PactNet;
using PactNet.Matchers;

namespace Consumer.IntegrationTests;

public class ProductClientTests
{
    private readonly HttpClient _httpClient;
    private readonly ProductClient _productClient;
    private readonly IPactBuilderV4 _pactBuilder = Pact
        .V4("My Consumer Service", "Product API", new PactConfig())
        .WithHttpInteractions();

    public ProductClientTests()
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        _productClient = new ProductClient(_httpClient);
    }
    
    [Fact]
    public async Task When_Product_Exists_Then_Api_Returns_Product()
    {
        var expectedProduct = new Product(1, "A cool product", 10.50, "Cool Store #12345");
        
        _pactBuilder
            .UponReceiving("A GET request to retrieve a product")
            .Given("There is a product with id 1", new Dictionary<string, string>
            {
                { "Name", "My product" },
                { "Location", "My store"}
            })
            .WithRequest(HttpMethod.Get, "/product/1")
            .WithHeader("Accept", "application/json")
            .WillRespond()
            .WithStatus(HttpStatusCode.OK)
            .WithHeader("Content-Type", "application/json; charset=utf-8")
            .WithJsonBody(new
            {
                Id = 1,
                Name = "A cool product",
                Price = Match.Decimal(10.50),
                Location = "Cool Store #12345"
            });
        
        await _pactBuilder.VerifyAsync(async context =>
        {
            _httpClient.BaseAddress = context.MockServerUri;
            var actualProduct = await _productClient.GetProduct(1);
            actualProduct.Should().BeEquivalentTo(expectedProduct);
        });
    }
}
