using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Provider.Api;
using Provider.Database;

namespace Provider.ContractTests;

public class ProviderState
{
    [property: JsonPropertyName("action")]
    public string Action { get; set; }
    [property: JsonPropertyName("params")]
    public Dictionary<string, string> Params { get; set; }
    [property: JsonPropertyName("state")]
    public string State { get; set; }
}

public class ProviderStateMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IProductService _productService;
    private readonly IDictionary<string, Action> _providerStates;
    
    public ProviderStateMiddleware(RequestDelegate next, IProductService productService)
    {
        _next = next;
        _productService = productService;
        _providerStates = new Dictionary<string, Action>
        {
            {
                "There is a product with id 1",
                () => MockData(1)
            }
        };
    }
    
    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Path.Value == "/provider-states")
        {
            await HandleProviderStateRequest(context);
            await context.Response.WriteAsync(string.Empty);
        }
        else
        {
            await _next(context);
        }
    }
    
    private async Task HandleProviderStateRequest(HttpContext context)
    {
        context.Response.StatusCode = (int)HttpStatusCode.OK;
        
        if (context.Request.Method.ToUpper() == HttpMethod.Post.ToString().ToUpper())
        {
            string body;
            using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8))
            {
                body = await reader.ReadToEndAsync();
            }
            
            var providerState = JsonSerializer.Deserialize<ProviderState>(body);
            if (!string.IsNullOrEmpty(providerState?.State))
            {
                _providerStates[providerState.State].Invoke();
            }
        }
    }
    
    private void MockData(int id)
    {
        _productService.GetProduct(id)!.Returns(Task.FromResult(new Product
        {
            Id = id,
            Name = "A cool product",
            Price = 9.3,
            Location = "Cool Store #12345"
        }));
    }
}