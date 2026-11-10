using MyFirstApi.Models;

namespace MyFirstApi.Services
{
    public interface IProductService
    {
        IEnumerable<Product> GetAll();
        Product? GetById(int id);
        IEnumerable<Product> Search(string query);
        Product Add(Product newProduct);
        bool Update(int id, Product newProduct);
        bool Delete(int id);
    }
}
