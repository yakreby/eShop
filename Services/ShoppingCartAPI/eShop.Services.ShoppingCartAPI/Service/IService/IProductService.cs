using eShop.Services.ShoppingCartAPI.Models.Dto;

namespace eShop.Services.ShoppingCartAPI.Service.IService
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProductsAsync();
    }
}
