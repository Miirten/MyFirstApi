using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using MyFirstApi.Auth;
using MyFirstApi.DTOs;
using MyFirstApi.Models;
using MyFirstApi.Services;
using MyFirstApi.Settings;

namespace MyFirstApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableRateLimiting("fixed")]
    public class ProductsController : ControllerBase
    {
        [Authorize(AuthenticationSchemes = ApiKeyAuthenticationHandler.SchemeName)]
        [HttpGet("secure")]
        public IActionResult GetSecureInfo()
        {
            return Ok(new { message = "You reached a protected endpoint." });
        }

        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        private readonly StoreSettings _storeSettings;

        public ProductsController(
            IProductService productService,
            IMapper mapper,
            IOptions<StoreSettings> options
        )
        {
            _productService = productService;
            _mapper = mapper;
            _storeSettings = options.Value; // bound settings object
        }

        [HttpGet("store-info")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult GetStoreInfo()
        {
            return Ok(
                new
                {
                    name = _storeSettings.StoreName,
                    currency = _storeSettings.DefaultCurrency,
                    discountsEnabled = _storeSettings.EnableDiscounts,
                }
            );
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ProductReadDto>))]
        public ActionResult<IEnumerable<ProductReadDto>> GetAll() =>
            Ok(_mapper.Map<IEnumerable<ProductReadDto>>(_productService.GetAll()));

        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ProductReadDto>))]
        public ActionResult<IEnumerable<ProductReadDto>> Search([FromQuery] string query) =>
            Ok(_mapper.Map<IEnumerable<ProductReadDto>>(_productService.Search(query)));

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductReadDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ProductReadDto> GetById(int id)
        {
            var product = _productService.GetById(id);

            if (product == null)
            {
                return NotFound(new { message = "Product not found" });
            }

            return _mapper.Map<ProductReadDto>(product);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ProductReadDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<ProductReadDto> Add([FromBody] ProductCreateDto newProductDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = _mapper.Map<Product>(newProductDto);
            var created = _productService.Add(product);

            var readDto = _mapper.Map<ProductReadDto>(created);

            return CreatedAtAction(nameof(GetById), new { id = readDto.Id }, readDto);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult Update(int id, [FromBody] ProductCreateDto updatedProductDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (_productService.Update(id, _mapper.Map<Product>(updatedProductDto)))
            {
                return NoContent();
            }

            return BadRequest();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult Delete(int id)
        {
            var deleted = _productService.Delete(id);

            if (!deleted)
            {
                return NotFound(new { message = "Product not found" });
            }

            return NoContent();
        }
    }
}
