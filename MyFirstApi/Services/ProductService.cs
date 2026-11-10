using Microsoft.Extensions.Caching.Memory;
using MyFirstApi.Models;

namespace MyFirstApi.Services
{
    public class ProductService : IProductService
    {
        private readonly IMemoryCache _cache;

        private const string ProductsCacheKey = "products_list";

        private readonly List<Product> _products = new()
        {
            new Product
            {
                Id = 1,
                Name = "Laptop",
                Price = 999.99m,
                Category = "Electronics",
            },
            new Product
            {
                Id = 2,
                Name = "Smartphone",
                Price = 699.99m,
                Category = "Electronics",
            },
            new Product
            {
                Id = 3,
                Name = "Headphones",
                Price = 199.99m,
                Category = "Accessories",
            },
        };

        public ProductService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public IEnumerable<Product> GetAll()
        {
            if (
                _cache.TryGetValue(ProductsCacheKey, out List<Product>? cachedProducts)
                && cachedProducts is not null
            )
            {
                return cachedProducts;
            }

            var products = _products.ToList();

            var cacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(
                TimeSpan.FromSeconds(30)
            );

            _cache.Set(ProductsCacheKey, products, cacheOptions);

            return products;
        }

        public Product? GetById(int id) => _products.FirstOrDefault(p => p.Id == id);

        public IEnumerable<Product> Search(string query) =>
            _products.Where(p => p.Name.Contains(query));

        public Product Add(Product newProduct)
        {
            newProduct.Id = _products.Select(p => p.Id).DefaultIfEmpty(0).Max() + 1;

            _products.Add(newProduct);

            _cache.Remove(ProductsCacheKey);

            return newProduct;
        }

        public bool Update(int id, Product updateProduct)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return false;
            }

            product.Name = updateProduct.Name;
            product.Category = updateProduct.Category;
            product.Price = updateProduct.Price;

            _cache.Remove(ProductsCacheKey);

            return true;
        }

        public bool Delete(int id)
        {
            var product = GetById(id);

            if (product == null)
            {
                return false;
            }

            var deleted = _products.Remove(product);

            if (deleted)
            {
                _cache.Remove(ProductsCacheKey);
            }

            return deleted;
        }
    }
}
