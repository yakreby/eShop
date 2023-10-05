using AutoMapper;
using eShop.Services.ShoppingCartAPI.Models;
using eShop.Services.ShoppingCartAPI.Models.Dto;

namespace eShop.Services.ShoppingCartAPI.Settings
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<CartHeader, CartHeaderDto>().ReverseMap();
                config.CreateMap<CartDetails, CartDetailsDto>().ReverseMap();
            });
            return mappingConfig;
        }
    }
}
