using EatEat.Services.ProductAPI.Helpers;
using EatEat.Services.ProductAPI.Models.Dto;
using EatEat.Services.ProductAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace EatEat.Services.ProductAPI.Controllers
{
   // [Authorize]
    [ApiController]
    [Route("api/Product")]
    public class ProductController : Controller
    {
        private ResponseDto _response;
        private IProductService _productService;

        public ProductController(IProductService productService)
        {
            _response = new ();
            _productService = productService;
        }

        [HttpGet]
        public async Task<ResponseDto> GetAllProducts()
        {
            try
            {
                _response.Result = await _productService.GetAllProductsAsync();
            }
            catch(Exception ex)
            {
                _response.Result = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        [HttpGet]
        [Route("{ID:int}")]
        public async Task<ResponseDto> GetProduct(int ID)
        {
            try
            {
                _response.Result = await _productService.GetProductByIDAsync(ID);
            }
            catch (Exception ex)
            {
                _response.Result = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ResponseDto> Create(ProductDto dto)
        {
            try
            {
                var result = await _productService.CreateProductAsync(dto);

                if (dto.Image != null)
                {
                    var fileName = result.ID + Path.GetExtension(dto.Image.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ProductImages", fileName);
                    using var fileStream = new FileStream(filePath, FileMode.Create);

                    dto.Image.CopyTo(fileStream);

                    var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";

                    await _productService.SaveProductImage(result, $"{baseUrl}/ProductImages/{fileName}", filePath);
                }
                else
                {
                    await _productService.SaveProductImage(result, null, "https://placehold.co/600x400");
                }

                _response.Result = result;
            }
            catch (Exception ex)
            {
                _response.Result = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        [HttpPut]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ResponseDto> Update(ProductDto dto)
        {
            try
            {
                var result = await _productService.UpdateProductAsync(dto);

                if (dto.Image != null)
                {
                    var fileName = result.ID + Path.GetExtension(dto.Image.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ProductImages", fileName);
                    using var fileStream = new FileStream(filePath, FileMode.Create);

                    dto.Image.CopyTo(fileStream);

                    var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";

                    await _productService.SaveProductImage(result, $"{baseUrl}/ProductImages/{fileName}", filePath);
                }

                _response.Result = result;
            }
            catch (Exception ex)
            {
                _response.Result = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ResponseDto> Delete(int ID)
        {
            try
            {
                await _productService.DeleteProductAsync(ID);
                _response.Result = true;
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
