using System.ComponentModel.DataAnnotations;

namespace MyFirstApi.DTOs
{
    public class ProductCreateDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Range(0.01, 10000)]
        public decimal Price { get; set; }

        [Required]
        public string Category { get; set; } = string.Empty;
    }
}
