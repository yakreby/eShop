using AutoMapper;
using eShop.Services.ProductAPI.Data;
using eShop.Services.ProductAPI.Models;
using eShop.Services.ProductAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eShop.Services.ProductAPI.Controllers
{
    [Route("api/product")]
    [ApiController]
    [Authorize]
    public class ProductAPIController : ControllerBase
    {
        private readonly AppDbContext _context;
        private ResponseDto _response;
        private IMapper _mapper;

        public ProductAPIController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _response = new ResponseDto();
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public ResponseDto Get()
        {
            try
            {
                IEnumerable<Product> productList = _context.Products.ToList();
                _response.Result = _mapper.Map<IEnumerable<ProductDto>>(productList);
                if (productList == null)
                {
                    _response.IsSuccess = false;
                }
                _response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpGet]
        [Route("{id:int}")]
        [ProducesResponseType(200)]
        [AllowAnonymous]
        public ResponseDto Get(int id)
        {
            try
            {
                Product product = _context.Products.First(x => x.ProductId == id);
                _response.Result = _mapper.Map<ProductDto>(product);
                if (product == null)
                {
                    _response.IsSuccess = false;
                }
                _response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }


        [HttpGet]
        [Route("GetByName/{productName}")]
        [ProducesResponseType(200)]
        [AllowAnonymous]
        public ResponseDto GetByName(string productName)
        {
            try
            {
                Product product = _context.Products.FirstOrDefault(x => x.Name.ToLower() == productName.ToLower());
                _response.Result = _mapper.Map<ProductDto>(product);
                if (product == null)
                {
                    _response.IsSuccess = false;
                }
                _response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPost]
        public ResponseDto Post([FromBody] ProductDto? productDto)
        {
            try
            {
                Product product = _mapper.Map<Product>(productDto);
                _context.Products.Add(product);
                _context.SaveChanges();

                if (product == null)
                {
                    _response.IsSuccess = false;
                }
                _response.Result = _mapper.Map<ProductDto>(product);
                _response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPut]
        public ResponseDto Put([FromBody] ProductDto productDto)
        {
            try
            {
                Product product = _mapper.Map<Product>(productDto);
                _context.Products.Update(product);
                _context.SaveChanges();

                if (product == null)
                {
                    _response.IsSuccess = false;
                }
                _response.Result = _mapper.Map<ProductDto>(product);
                _response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpDelete]
        [Route("{id:int}")]
        public ResponseDto Delete(int id)
        {
            try
            {
                var product = _context.Products.FirstOrDefault(x => x.ProductId == id);
                _context.Products.Remove(product);
                _context.SaveChanges();

                if (product == null)
                {
                    _response.IsSuccess = false;
                }
                _response.Result = _mapper.Map<ProductDto>(product);
                _response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
    }
}