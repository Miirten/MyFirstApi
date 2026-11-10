using AutoMapper;
using MyFirstApi.DTOs;
using MyFirstApi.Models;

namespace MyFirstApi.Profiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductReadDto>();

            CreateMap<ProductCreateDto, Product>();
        }
    }
}
