using AutoMapper;
using EatEat.Services.ShoppingCartAPI.Data;
using EatEat.Services.ShoppingCartAPI.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace EatEat.Services.ShoppingCartAPI.Controllers
{
    [ApiController]
    [Route("api/Cart")]
    public class CartAPIController : Controller
    {
        private ResponseDto _response;
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public CartAPIController(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
            _response = new();
        }

        [HttpPost("Upsert")]
        public async Task<ResponseDto> Upsert(CartDto dto)
        {
            try
            {
                _response.Result = await _productService.GetAllProductsAsync();
            }
            catch (Exception ex)
            {
                _response.Result = false;
                _response.Message = ex.Message;
            }

            return _response;
        }
    }
}