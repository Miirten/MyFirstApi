using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyFirstApi.Models;
using MyFirstApi.Services;

namespace MyFirstApi.Tests
{
    [TestClass]
    public class ProductServiceTests
    {
        private ProductService _service;

        [TestInitialize]
        public void Setup()
        {
            _service = new ProductService(new MemoryCache(new MemoryCacheOptions()));
        }

        [TestMethod]
        public void GetAll_WhenCalled_ReturnsProducts()
        {
            var products = _service.GetAll();
            Assert.IsTrue(products.Any(), "Expected product list to contain items.");
        }

        [TestMethod]
        public void Add_WhenProductIsValid_IncreasesCount()
        {
            var initialCount = _service.GetAll().Count();

            _service.Add(
                new Product
                {
                    Name = "Keyboard",
                    Price = 49.99m,
                    Category = "Accessories",
                }
            );

            var newCount = _service.GetAll().Count();
            Assert.AreEqual(initialCount + 1, newCount);
        }

        [TestMethod]
        public void Delete_WhenProductExists_RemovesProduct()
        {
            var product = _service.Add(
                new Product
                {
                    Name = "Mouse",
                    Price = 29.99m,
                    Category = "Accessories",
                }
            );

            var result = _service.Delete(product.Id);

            Assert.IsTrue(result);
        }
    }
}
