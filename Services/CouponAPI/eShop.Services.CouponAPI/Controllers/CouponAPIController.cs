using AutoMapper;
using eShop.Services.CouponAPI.Data;
using eShop.Services.CouponAPI.Models;
using eShop.Services.CouponAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eShop.Services.CouponAPI.Controllers
{
    [Route("api/coupon")]
    [ApiController]
    [Authorize]
    public class CouponAPIController : ControllerBase
    {
        private readonly AppDbContext _context;
        private ResponseDto _response;
        private IMapper _mapper;

        public CouponAPIController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _response = new ResponseDto();
            _mapper = mapper;
        }

        [HttpGet]
        public ResponseDto Get()
        {
            try
            {
                IEnumerable<Coupon> couponList = _context.Coupons.ToList();
                _response.Result = _mapper.Map<IEnumerable<CouponDto>>(couponList);
                if (couponList == null)
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
        public ResponseDto Get(int id)
        {
            try
            {
                Coupon coupon = _context.Coupons.First(x => x.CouponId == id);
                _response.Result = _mapper.Map<CouponDto>(coupon);
                if (coupon == null)
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
        [Route("GetByCode/{code}")]
        [ProducesResponseType(200)]
        public ResponseDto GetByCode(string code)
        {
            try
            {
                Coupon coupon = _context.Coupons.FirstOrDefault(x => x.CouponCode.ToLower() == code.ToLower());
                _response.Result = _mapper.Map<CouponDto>(coupon);
                if (coupon == null)
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
        public ResponseDto Post([FromBody] CouponDto? couponDto)
        {
            try
            {
                Coupon coupon = _mapper.Map<Coupon>(couponDto);
                _context.Coupons.Add(coupon);
                _context.SaveChanges();

                if (coupon == null)
                {
                    _response.IsSuccess = false;
                }
                _response.Result = _mapper.Map<CouponDto>(coupon);
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
        public ResponseDto Put([FromBody] CouponDto couponDto)
        {
            try
            {
                Coupon coupon = _mapper.Map<Coupon>(couponDto);
                _context.Coupons.Update(coupon);
                _context.SaveChanges();

                if (coupon == null)
                {
                    _response.IsSuccess = false;
                }
                _response.Result = _mapper.Map<CouponDto>(coupon);
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
                var coupon = _context.Coupons.FirstOrDefault(x => x.CouponId == id);
                _context.Coupons.Remove(coupon);
                _context.SaveChanges();

                if (coupon == null)
                {
                    _response.IsSuccess = false;
                }
                _response.Result = _mapper.Map<CouponDto>(coupon);
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
