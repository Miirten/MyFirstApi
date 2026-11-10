using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyFirstApi.DTOs;

namespace MyFirstApi.Tests
{
    [TestClass]
    public class ProductsControllerTests
    {
        private WebApplicationFactory<Program> _factory;
        private HttpClient _client;

        [TestInitialize]
        public void Setup()
        {
            _factory = new WebApplicationFactory<Program>();
            _client = _factory.CreateClient();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        [TestMethod]
        public async Task GetAllProducts_WhenCalled_ReturnsOkAndList()
        {
            var response = await _client.GetAsync("/api/products");
            response.EnsureSuccessStatusCode();

            var products = await response.Content.ReadFromJsonAsync<List<ProductReadDto>>();

            Assert.IsNotNull(products);
            Assert.IsTrue(products.Count > 0);
        }

        [TestMethod]
        public async Task AddProduct_WhenValid_ReturnsCreated()
        {
            var newProduct = new
            {
                Name = "Webcam",
                Price = 89.99,
                Category = "Electronics",
            };

            var response = await _client.PostAsJsonAsync("/api/products", newProduct);

            Assert.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode);
        }

        [TestMethod]
        public async Task AddProduct_WhenInvalid_ReturnsBadRequest()
        {
            var invalidProduct = new { Name = "", Price = -5 };

            var response = await _client.PostAsJsonAsync("/api/products", invalidProduct);

            Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
