using AutoMapper;
using eShop.Services.ShoppingCartAPI.Data;
using eShop.Services.ShoppingCartAPI.Models;
using eShop.Services.ShoppingCartAPI.Models.Dto;
using eShop.Services.ShoppingCartAPI.RabbitMQSender;
using eShop.Services.ShoppingCartAPI.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eShop.Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartAPIController : ControllerBase
    {
        private readonly AppDbContext _context;
        private ResponseDto _response;
        private IMapper _mapper;
        private IConfiguration _configuration;
        private readonly IProductService _productService;
        private readonly ICouponService _couponService;
        private readonly IRabbitMQCartMessageSender _rabbitmqCartMessageSender;
        public CartAPIController(AppDbContext context, IMapper mapper, IProductService productService, ICouponService couponService,
            IRabbitMQCartMessageSender rabbitMQCartMessageSender, IConfiguration configuration)
        {
            _context = context;
            _response = new ResponseDto();
            _mapper = mapper;
            _productService = productService;
            _couponService = couponService;
            _rabbitmqCartMessageSender = rabbitMQCartMessageSender;
            _configuration = configuration;
        }

        [HttpGet("GetCart/{userId}")]
        public async Task<ResponseDto> GetCart(string userId)
        {
            try
            {
                CartDto cart = new()
                {
                    CartHeader = _mapper.Map<CartHeaderDto>(_context.CartHeaders.First(
                        x => x.UserId == userId))
                };
                cart.CartDetails = _mapper.Map<IEnumerable<CartDetailsDto>>(_context.CartDetails.Where(
                    x => x.CartHeaderId == cart.CartHeader.CartHeaderId));

                IEnumerable<ProductDto> productDtos = await _productService.GetProductsAsync();

                foreach (var item in cart.CartDetails)
                {
                    item.Product = productDtos.FirstOrDefault(x => x.ProductId == item.ProductId);
                    cart.CartHeader.CartTotal += item.Count * item.Product.Price;
                }

                if (!string.IsNullOrEmpty(cart.CartHeader.CouponCode))
                {
                    CouponDto coupon = await _couponService.GetCouponAsync(cart.CartHeader.CouponCode);
                    if (coupon != null && cart.CartHeader.CartTotal > coupon.MinAmount)
                    {
                        cart.CartHeader.CartTotal -= coupon.DiscountAmount;
                        cart.CartHeader.Discount = coupon.DiscountAmount;
                    }
                }
                _response.Result = cart;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }
            return _response;
        }

        [HttpPost("ApplyCoupon")]
        public async Task<object> ApplyCoupon([FromBody] CartDto cart)
        {
            try
            {
                var cartFromDb = await _context.CartHeaders.FirstAsync(x => x.UserId == cart.CartHeader.UserId);
                cartFromDb.CouponCode = cart.CartHeader.CouponCode;
                _context.CartHeaders.Update(cartFromDb);
                await _context.SaveChangesAsync();
                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpDelete("RemoveCoupon")]
        public async Task<object> RemoveCoupon([FromBody] CartDto cart)
        {
            try
            {
                var cartFromDb = await _context.CartHeaders.FirstAsync(x => x.UserId == cart.CartHeader.UserId);
                cartFromDb.CouponCode = cart.CartHeader.CouponCode;
                _context.CartHeaders.Update(cartFromDb);
                await _context.SaveChangesAsync();
                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPost("CartUpsert")]
        public async Task<ResponseDto> CartUpsert([FromBody] CartDto cart)
        {
            try
            {
                var cartHeaderFromDb = await _context.CartHeaders.AsNoTracking().FirstOrDefaultAsync(
                    x => x.UserId == cart.CartHeader.UserId);
                if (cartHeaderFromDb == null)
                {
                    //Create Headers and Details
                    CartHeader cartHeader = _mapper.Map<CartHeader>(cart.CartHeader);
                    _context.CartHeaders.Add(cartHeader);
                    await _context.SaveChangesAsync();
                    cart.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
                    _context.CartDetails.Add(_mapper.Map<CartDetails>(cart.CartDetails.First()));
                    await _context.SaveChangesAsync();
                }
                else
                {
                    //Check if details has same product
                    var cartDetailsFromDb = await _context.CartDetails.AsNoTracking().FirstOrDefaultAsync(
                        x => x.ProductId == cart.CartDetails.First().ProductId &&
                        x.CartHeaderId == cartHeaderFromDb.CartHeaderId);
                    if (cartDetailsFromDb == null)
                    {
                        //Create CartDetails
                        cart.CartDetails.First().CartHeaderId = cartHeaderFromDb.CartHeaderId;
                        _context.CartDetails.Add(_mapper.Map<CartDetails>(cart.CartDetails.First()));
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        //Update Count in the Cart Details
                        cart.CartDetails.First().Count += cartDetailsFromDb.Count;
                        cart.CartDetails.First().CartHeaderId = cartDetailsFromDb.CartHeaderId;
                        cart.CartDetails.First().CartDetailsId = cartDetailsFromDb.CartDetailsId;
                        _context.CartDetails.Update(_mapper.Map<CartDetails>(cart.CartDetails.First()));
                        await _context.SaveChangesAsync();
                    }
                }
                _response.Result = cart;
                return _response;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message.ToString();
                _response.IsSuccess = false;
                return _response;
            }
        }

        [HttpDelete("RemoveCart")]
        public async Task<ResponseDto> RemoveCart([FromBody] int cartDetailsId)
        {
            try
            {
                CartDetails cartDetails = _context.CartDetails.First(
                    x => x.CartDetailsId == cartDetailsId);

                int totalCountOfCartItem = _context.CartDetails.Where(
                    x => x.CartHeaderId == cartDetails.CartHeaderId).Count();
                _context.CartDetails.Remove(cartDetails);
                if (totalCountOfCartItem == 1)
                {
                    var cartHeaderToRemove = await _context.CartHeaders
                        .FirstOrDefaultAsync(x => x.CartHeaderId == cartDetails.CartHeaderId);
                    _context.CartHeaders.Remove(cartHeaderToRemove);
                }
                await _context.SaveChangesAsync();
                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message.ToString();
                _response.IsSuccess = false;
                return _response;
            }
            return _response;
        }

        [HttpPost("EmailCartRequest")]
        public async Task<object> EmailCartRequest([FromBody] CartDto cartDto)
        {
            try
            {
                _rabbitmqCartMessageSender.SendMessage(cartDto, _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue"));
                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.ToString();
            }
            return _response;
        }
    }
}
