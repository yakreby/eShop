using eShop.Web.Models.Dto;
using eShop.Web.Service.Base;

namespace eShop.Web.Service.IService
{
    public interface IProductService : IScopedService
    {
        Task<ResponseDto> GetProductAsync(string productName);
        Task<ResponseDto> GetAllProductsAsync();
        Task<ResponseDto> GetProductByIdAsync(int id);
        Task<ResponseDto> CreateProductAsync(ProductDto productDto);
        Task<ResponseDto> UpdateProductAsync(ProductDto productDto);
        Task<ResponseDto> DeleteProductAsync(int id);
    }
}
