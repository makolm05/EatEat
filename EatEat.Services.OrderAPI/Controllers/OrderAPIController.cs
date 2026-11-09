using AutoMapper;
using EatEat.MessageBus;
using Microsoft.AspNetCore.Mvc;
using EatEat.Services.OrderAPI.Data;
using Microsoft.AspNetCore.Authorization;
using EatEat.Services.OrderAPI.Models.Dto;
using EatEat.Services.OrderAPI.Services.IService;
using EatEat.Services.OrderAPI.Helpers;
using EatEat.Services.OrderAPI.Models;
using Stripe;
using Stripe.Checkout;
using Microsoft.EntityFrameworkCore;

namespace EatEat.Services.OrderAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/Order")]
    public class OrderAPIController : Controller
    {
        private ResponseDto _response;
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;
        private readonly IProductService _productService;

        public OrderAPIController(AppDbContext db, IConfiguration configuration, IMessageBus messageBus, IProductService productService, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
            _response = new();
            _messageBus = messageBus;
            _configuration = configuration;
            _productService = productService;
        }

        [HttpGet]
        public ResponseDto? Get(int orderID)
        {
            try
            {
                var orderHeader = _db.OrderHeaders.Include(x => x.OrderDetails).First(x => x.ID == orderID);
                _response.Result = _mapper.Map<OrderHeaderDto>(orderHeader);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }


        [HttpGet("GetAllOrders")]
        public ResponseDto? Get(string? userID = "")
        {
            try
            {
                IEnumerable<OrderHeader> orderHeaders;
                if (User.IsInRole(Roles.Admin))
                {
                    orderHeaders = _db.OrderHeaders.Include(x => x.OrderDetails).OrderByDescending(x => x.ID).ToList();
                }
                else
                {
                    orderHeaders = _db.OrderHeaders.Include(x => x.OrderDetails).Where(x => x.UserID == userID).OrderByDescending(x => x.ID).ToList();
                }
                _response.Result = _mapper.Map<IEnumerable<OrderHeaderDto>>(orderHeaders);

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        [HttpPost]
        public async Task<ResponseDto> Create([FromBody] CartDto dto)
        {
            try
            {
                var orderHeaderDto = _mapper.Map<OrderHeaderDto>(dto.CartHeader);
                orderHeaderDto.OrderTime = DateTime.Now;
                orderHeaderDto.Status = StaticDetails.StatusPending;

                orderHeaderDto.OrderDetails = _mapper.Map<IEnumerable<OrderDetailsDto>>(dto.CartDetails);

                var orderHeader = (await _db.OrderHeaders.AddAsync(_mapper.Map<OrderHeader>(orderHeaderDto))).Entity;
                await _db.SaveChangesAsync();

                orderHeaderDto.ID = orderHeader.ID;
                _response.Result = orderHeaderDto;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        [HttpPost("UpdateOrderStatus/{orderID:int}")]
        public async Task<ResponseDto> UpdateOrderStatus(int orderID, [FromBody] string status)
        {
            try
            {
                var orderHeader = _db.OrderHeaders.First(x => x.ID == orderID);
                if(status == StaticDetails.StatusCancelled)
                {
                    var options = new RefundCreateOptions
                    {
                        Reason = RefundReasons.RequestedByCustomer,
                        PaymentIntent = orderHeader.PaymentIntentID
                    };

                    var service = new RefundService();
                    var refund = service.Create(options);
                }

                orderHeader.Status = status;
                await _db.SaveChangesAsync();
                _response.Result = _mapper.Map<OrderHeaderDto>(orderHeader);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPost("CreateStripeSession")]
        public async Task<ResponseDto> CreateStripeSession([FromBody] StripeRequestDto dto)
        {
            try
            {
                var options = new SessionCreateOptions
                {
                    SuccessUrl = dto.ApprovedUrl,
                    CancelUrl = dto.CancelUrl,
                    LineItems = new(),
                    Mode = "payment"
                };

                var discounts = new List<SessionDiscountOptions>()
                {
                    new SessionDiscountOptions
                    {
                        Coupon = dto.OrderHeader.CouponCode
                    }
                };

                foreach (var item in dto.OrderHeader.OrderDetails)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.ProductPrice * 100), // Convert to cents
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Name,
                                Description = item.Product.Description
                            }
                        },
                        Quantity = item.Count
                    };

                    options.LineItems.Add(sessionLineItem);
                }

                if(dto.OrderHeader.Discount > 0)
                {
                    options.Discounts = discounts;
                }

                var service = new SessionService();
                var session = await service.CreateAsync(options);

                dto.StripeSessionID = session.Id;
                dto.StripeSessionUrl = session.Url;

                var orderHeader = _db.OrderHeaders.First(x => x.ID == dto.OrderHeader.ID);
                orderHeader.StripeSessionID = session.Id;
                await _db.SaveChangesAsync();

                _response.Result = dto;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        [HttpPost("ValidateStripeSession")]
        public async Task<ResponseDto> ValidateStripeSession([FromBody] int orderHeaderID)
        {
            try
            {
                var orderHeader = _db.OrderHeaders.First(x => x.ID == orderHeaderID);

                var service = new SessionService();
                var session = await service.GetAsync(orderHeader.StripeSessionID);

                var paymentIntent = new PaymentIntentService().Get(session.PaymentIntentId);

                if(paymentIntent.Status == "succeeded")
                {
                    orderHeader.PaymentIntentID = paymentIntent.Id;
                    orderHeader.Status = StaticDetails.StatusApproved;
                    await _db.SaveChangesAsync();

                    var rewards = new RewardsDto
                    {
                        OrderID = orderHeader.ID,
                        UserID = orderHeader.UserID,
                        RewardsActivity = (int)orderHeader.OrderTotal
                    };

                    string topicName = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
                    await _messageBus.PublishMessage(rewards, topicName);

                    _response.Result = _mapper.Map<OrderHeaderDto>(orderHeader);
                }

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