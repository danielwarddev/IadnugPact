using System.Net;
using System.Net.Http.Json;

namespace Consumer;

public record Product(int Id, string Name, double Price, string Location);

public interface IProductClient
{
    Task<Product?> GetProduct(int productId);
}

public class ProductClient : IProductClient
{
    private readonly HttpClient _client;

    public ProductClient(HttpClient client)
    {       
        _client = client;
    }

    public async Task<Product?> GetProduct(int productId)
    {
        Product? product;
        try
        {
            product = await _client.GetFromJsonAsync<Product?>($"/product/{productId}");
        }
        catch (HttpRequestException e) when (e.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
        return product;
    }
}