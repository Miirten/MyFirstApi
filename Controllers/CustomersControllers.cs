using Microsoft.AspNetCore.Mvc;

namespace MyFirstApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private static readonly List<string> Customers = new() { "Alice", "Bob", "Charlie" };

        [HttpGet]
        public IEnumerable<string> GetAllCustomers()
        {
            return Customers;
        }
    }
}
