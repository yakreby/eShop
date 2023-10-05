using eShop.Web.Models.Dto;
using eShop.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace eShop.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> ProductIndex()
        {
            List<ProductDto>? list = new();
            ResponseDto response = await _productService.GetAllProductsAsync();
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["Error"] = response?.Message;
            }
            return View(list);
        }

        public async Task<IActionResult> CreateProduct()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductDto productDto)
        {
            if (ModelState.IsValid)
            {
                ResponseDto? response = await _productService.CreateProductAsync(productDto);
                if (response != null && response.IsSuccess)
                {
                    TempData["Success"] = "Product Created";
                    return RedirectToAction(nameof(ProductIndex));
                }
                else
                {
                    TempData["Error"] = response?.Message;
                }
            }
            return View(productDto);
        }

        public async Task<IActionResult> UpdateProduct(int productId)
        {
            ResponseDto response = await _productService.GetProductByIdAsync(productId);
            var product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProduct(ProductDto productDto)
        {
            if (ModelState.IsValid)
            {
                ResponseDto? response = await _productService.UpdateProductAsync(productDto);
                if (response != null && response.IsSuccess)
                {
                    TempData["Success"] = "Product Updated";
                    return RedirectToAction(nameof(ProductIndex));
                }
                else
                {
                    TempData["Error"] = response?.Message;
                }
            }
            return View(productDto);
        }

        public async Task<IActionResult> DeleteProduct(int productId)
        {
            ResponseDto? response = await _productService.DeleteProductAsync(productId);
            if (response != null && response.IsSuccess)
            {
                TempData["Success"] = "Product Deleted";
                return RedirectToAction(nameof(ProductIndex));
            }
            else
            {
                TempData["Error"] = response?.Message;
            }
            return RedirectToAction(nameof(ProductIndex));
        }
    }
}
