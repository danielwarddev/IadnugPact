using Microsoft.EntityFrameworkCore;
using Provider.Database;

namespace Provider.Api;

public interface IProductService
{
    Task<Product?> GetProduct(int productId);
}

public class ProductService : IProductService
{
    private readonly ProductContext _context;

    public ProductService(ProductContext context)
    {
        _context = context;
    }
    
    public async Task<Product?> GetProduct(int productId)
    {
        return await _context.Products.FirstOrDefaultAsync(x => x.Id == productId);
    }
}