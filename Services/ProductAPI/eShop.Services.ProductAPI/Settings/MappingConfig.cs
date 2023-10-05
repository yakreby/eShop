using AutoMapper;
using eShop.Services.ProductAPI.Models;
using eShop.Services.ProductAPI.Models.Dto;

namespace eShop.Services.ProductAPI.Settings
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<ProductDto, Product>();
                config.CreateMap<Product, ProductDto>();
                config.CreateMap<ProductUpdateDto, Product>().ReverseMap();
            });
            return mappingConfig;
        }
    }
}
