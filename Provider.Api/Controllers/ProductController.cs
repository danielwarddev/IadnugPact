using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace Provider.Api.Controllers;

//public record Product(int Id, string Name, double Price, string Location);
public record ProductDto(
    [property: JsonPropertyName("Id")] int Id,
    [property: JsonPropertyName("Name")] string Name,
    [property: JsonPropertyName("Price")] double Price,
    [property: JsonPropertyName("Location")] string Location
);

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet("/product/{productId:int}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int productId)
    {
        var product = await _productService.GetProduct(productId);
        if (product == null)
        {
            return NotFound();
        }

        var productDto = new ProductDto(product.Id, product.Name, product.Price, product.Location);
        return Ok(productDto);
    }
}