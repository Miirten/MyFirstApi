using Microsoft.AspNetCore.Mvc;

namespace MyFirstApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private static readonly List<string> Products = new List<string>
        {
            "Laptop",
            "Smartphone",
            "Headphones",
        };

        [HttpGet]
        public ActionResult<string> GetAllProducts()
        {
            return Ok(Products);
        }

        [HttpGet("{id}")]
        public ActionResult<string> GetProductById(int id)
        {
            if (id < 0 || id >= Products.Count)
            {
                return NotFound("Product not found");
            }
            return Products[id];
        }

        [HttpPost]
        public ActionResult AddProduct([FromBody] string newProduct)
        {
            Products.Add(newProduct);
            return CreatedAtAction(
                nameof(GetProductById),
                new { id = Products.Count - 1 },
                newProduct
            );
        }

        [HttpPut("{id}")]
        public ActionResult UpdateProduct(int id, [FromBody] string updatedProduct)
        {
            if (id < 0 || id >= Products.Count)
            {
                return NotFound("Product not found");
            }

            Products[id] = updatedProduct;
            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteProduct(int id)
        {
            if (id < 0 || id >= Products.Count)
            {
                return NotFound("Product not found");
            }

            Products.RemoveAt(id);
            return NoContent();
        }

        [HttpGet("search")]
        public IEnumerable<string> SearchProducts([FromQuery] string keyword)
        {
            return Products.Where(p => p.Contains(keyword, StringComparison.OrdinalIgnoreCase));
        }
    }
}
