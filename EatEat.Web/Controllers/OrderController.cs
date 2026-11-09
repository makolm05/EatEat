using Newtonsoft.Json;
using EatEat.Web.Utility;
using EatEat.Web.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using EatEat.Web.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;

namespace EatEat.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string status)
        {
            var userID = string.Empty;
            var list = new List<OrderHeaderDto>();
            if (!User.IsInRole(StaticDetails.AdminRole))
            {
                userID = User.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value;
            }

            var response = await _orderService.GetAllOrdersAsync(userID);
            if(response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<OrderHeaderDto>>(response.Result.ToString());
                switch (status)
                {
                    case "approved":
                        list = list.Where(x => x.Status == StaticDetails.StatusApproved).ToList();
                        break;
                    case "readyforpickup":
                        list = list.Where(x => x.Status == StaticDetails.StatusReadyForPickup).ToList();
                        break;
                    case "cancelled":
                        list = list.Where(x => x.Status == StaticDetails.StatusCancelled).ToList();
                        break;
                    default:
                        break;
                }
            }

            return Json(new
            {
                data = list
            });
        }

        public async Task<IActionResult> OrderDetail(int orderId)
        {
            var orderHeader = new OrderHeaderDto();
            string userID = User.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value;

            var response = await _orderService.GetOrderAsync(orderId);
            if (response != null && response.IsSuccess)
            {
                orderHeader = JsonConvert.DeserializeObject<OrderHeaderDto>(response.Result.ToString());
            }

            if(!User.IsInRole(StaticDetails.AdminRole) && userID != orderHeader.UserID)
            {
                return NotFound();
            }
            
            return View(orderHeader);
        }


        [HttpPost("ChangeStatus")]
        public async Task<IActionResult> ChangeStatus(int orderId, string orderStatus)
        {
            var response = await _orderService.UpdateOrderStatusAsync(orderId, orderStatus);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Status updated successfully";
                return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
            }

            return View();
        }
    }
}
