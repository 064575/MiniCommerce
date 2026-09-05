using System.Text.Json;
using ProductService.Models;

namespace ProductService.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly string _filePath;

    public ProductRepository(IWebHostEnvironment environment)
    {
        _filePath = Path.Combine(
            environment.ContentRootPath,
            "Storage",
            "products.json"
        );
    }

    public async Task<List<Product>> GetAllAsync()
    {
        if (!File.Exists(_filePath))
        {
            return new List<Product>();
        }

        var json = await File.ReadAllTextAsync(_filePath);

        if (string.IsNullOrWhiteSpace(json))
        {
            return new List<Product>();
        }

        return JsonSerializer.Deserialize<List<Product>>(json)
               ?? new List<Product>();
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        var products = await GetAllAsync();

        return products.FirstOrDefault(x => x.Id == id);
    }

    public async Task AddAsync(Product product)
    {
        var products = await GetAllAsync();

        products.Add(product);

        await SaveAsync(products);
    }

    public async Task UpdateAsync(Product product)
    {
        var products = await GetAllAsync();

        var existingProduct = products.FirstOrDefault(x => x.Id == product.Id);

        if (existingProduct is null)
        {
            return;
        }

        existingProduct.Name = product.Name;
        existingProduct.Description = product.Description;
        existingProduct.Price = product.Price;
        existingProduct.IsActive = product.IsActive;

        await SaveAsync(products);
    }

    private async Task SaveAsync(List<Product> products)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var json = JsonSerializer.Serialize(products, options);

        await File.WriteAllTextAsync(_filePath, json);
    }
}