using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using EatEat.Services.ShoppingCartAPI.Data;
using EatEat.Services.ShoppingCartAPI.Models;
using EatEat.Services.ShoppingCartAPI.Models.Dto;
using EatEat.Services.ShoppingCartAPI.Services.IService;
using EatEat.MessageBus;

namespace EatEat.Services.ShoppingCartAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/Cart")]
    public class CartAPIController : Controller
    {
        private ResponseDto _response;
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;
        private readonly ICouponService _couponService;
        private readonly IProductService _productService;

        public CartAPIController(AppDbContext db, IConfiguration configuration, ICouponService couponService, IMessageBus messageBus, IProductService productService, IMapper mapper)
        {
            _db = db;
            _mapper = mapper; 
            _response = new();
            _messageBus = messageBus;
            _configuration = configuration;
            _couponService = couponService;
            _productService = productService;
        }

        [HttpGet("GetCart/{userID}")]
        public async Task<ResponseDto> GetCart(string userID)
        {
            try
            {
                var cart = new CartDto
                {
                    CartHeader = _mapper.Map<CartHeaderDto>(_db.CartHeaders.First(x => x.UserID == userID))
                };

                cart.CartDetails = _mapper.Map<IEnumerable<CartDetailsDto>>(_db.CartDetails
                    .Where(x => x.CartHeaderID == cart.CartHeader.ID).ToList());

                var products = await _productService.GetProductsAsync();
                foreach(var item in cart.CartDetails)
                {
                    item.Product = products.First(x => x.ID == item.ProductID);
                    cart.CartHeader.CartTotal += item.Count * item.Product.Price;
                }

                if (!string.IsNullOrWhiteSpace(cart.CartHeader.CouponCode))
                {
                    var coupon = await _couponService.GetCouponAsync(cart.CartHeader.CouponCode);
                    if(cart.CartHeader.CartTotal >= coupon.MinAmount)
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
        public async Task<ResponseDto> ApplyCoupon([FromBody] ApplyCouponDto dto)
        {
            try
            {
                var cart = await _db.CartHeaders.FirstAsync(x => x.UserID == dto.UserID);
                cart.CouponCode = dto.CouponCode; // if coupon not found logic
                await _db.SaveChangesAsync();

                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }

            return _response;
        }

        [HttpPost("EmailCartRequest")]
        public async Task<ResponseDto> EmailCartRequest([FromBody] CartDto dto)
        {
            try
            {
                await _messageBus.PublishMessage(dto, _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCart"));
                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }

            return _response;
        }

        [HttpPost("Upsert")]
        public async Task<ResponseDto> Upsert(CartDto dto)
        {
            try
            {
                var cartHeader = await _db.CartHeaders.AsNoTracking().FirstOrDefaultAsync(u => u.UserID == dto.CartHeader.UserID);
                if(cartHeader == null)
                {
                    // create header and details
                    var entityHeader = _mapper.Map<CartHeader>(dto.CartHeader);
                    _db.CartHeaders.Add(entityHeader);
                    await _db.SaveChangesAsync();

                    dto.CartDetails.First().CartHeaderID = entityHeader.ID;
                    _db.CartDetails.Add(_mapper.Map<CartDetails>(dto.CartDetails.First()));
                    await _db.SaveChangesAsync();
                }
                else
                {
                    var cartDetails = await _db.CartDetails.AsNoTracking().FirstOrDefaultAsync(x => x.ProductID == dto.CartDetails.First().ProductID && x.CartHeaderID == cartHeader.ID);
                    if(cartDetails == null)
                    {
                        dto.CartDetails.First().CartHeaderID = cartHeader.ID;
                        _db.CartDetails.Add(_mapper.Map<CartDetails>(dto.CartDetails.First()));
                        await _db.SaveChangesAsync();
                    }
                    else
                    {
                        //update count
                        dto.CartDetails.First().Count += cartDetails.Count;
                        dto.CartDetails.First().ID = cartDetails.ID;
                        dto.CartDetails.First().CartHeaderID = cartDetails.CartHeaderID;
                        _db.CartDetails.Update(_mapper.Map<CartDetails>(dto.CartDetails.First()));
                        await _db.SaveChangesAsync();
                    }
                }
                
                _response.Result = dto;
            }
            catch(Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }

            return _response;
        }

        [HttpPost("RemoveCartDetails")]
        public async Task<ResponseDto> RemveCartDetails([FromBody] int cartDetailsID)
        {
            try
            {
                var cartDetails = await _db.CartDetails.AsNoTracking().FirstAsync(x => x.ID == cartDetailsID);
                var totalCount = _db.CartDetails.Where(x => x.CartHeaderID == cartDetails.CartHeaderID).Count();

                _db.CartDetails.Remove(cartDetails);
                if (totalCount == 1)
                    _db.CartHeaders.Remove(await _db.CartHeaders.FirstAsync(x => x.ID == cartDetails.CartHeaderID));

                await _db.SaveChangesAsync();
                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }

            return _response;
        }
    }
}