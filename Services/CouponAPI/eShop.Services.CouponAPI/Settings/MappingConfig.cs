using AutoMapper;
using eShop.Services.CouponAPI.Models;
using eShop.Services.CouponAPI.Models.Dto;

namespace eShop.Services.CouponAPI.Settings
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<CouponDto, Coupon>();
                config.CreateMap<Coupon, CouponDto>();
            });
            return mappingConfig;
        }
    }
}
