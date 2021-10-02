using eShopSolution.Application.Catalog.Products;
using eShopSolution.Application.Catalog.Products.Dtos;
using eShopSolution.ViewModels.Catalog.Products;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eShopSolution.BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        /*tiêm thằng interface vào để gọi các abstract method
        * Vào startup.cs phần ConfigureServices
        * Declare DI
        * Chỉ dịnh khi khai báo DI Interface nó sẽ hiểu là tiêm thằng class impl interface đó
        * 
        * services.AddTransient(IPublicProductService,PublicProductService>();
        * AddTransient(thoáng qua) : có nghĩa là khi có req thì sẽ inject vào
        * 
        */

        private readonly IPublicProductService _publicProductService;
        private readonly IManageProductService _manageProductService;

        public ProductController(IPublicProductService publicProductService, IManageProductService manageProductService)
        {
            _publicProductService = publicProductService;
            _manageProductService = manageProductService;
        }

        // http://localhost:port/product
        [HttpGet]
        public async Task<IActionResult> Get() {

            var products = await _publicProductService.GetAll();
            return Ok(products);
        }

        // http://localhost:port/product/public-paging
        [HttpGet("public-paging")] // đặt bị danh cho url để ko bị trùng

        /*
         * FromQuery => thông số getPublicproductPagingRequest sẽ đc truyền vào thông qua parameter trên url
        */

        public async Task<IActionResult> GetProductById([FromQuery] GetPublicProductPagingRequest request)
        {

            var products = await _publicProductService.GetAllByCategoryId(request);
            return Ok(products);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductCreateRequest request)
        {

            var productId = await _manageProductService.Create(request); // trả về productId

            if (productId == 0)
            {
                return BadRequest(); // response lỗi 400 ko tạo dc
            }

            var product = await _manageProductService.GetById(productId, request.LanguageId);
            return CreatedAtAction (nameof(GetProductById), new { id = productId } ,product); 
            // trả về response code là 201
            // new {id = productId}  ==> route data to use for generating URL
        }


        // http:localhost:port/product/id
        [HttpGet("{productId}/{languageId}")]
        public async Task<IActionResult> GetProductById(int productId, String languageId)
        {

            var product = await _manageProductService.GetById(productId, languageId);

            if (product == null)
            {
                return BadRequest("Cannot find product"); // response lỗi 400 ko tìm thấy 
            }
            return Ok(product); // trả về response code là 201
        }


        [HttpPut]
        public async Task<IActionResult> UpdateProduct([FromBody]ProductUpdateRequest request)
        {

            var affectedRow = await _manageProductService.Update(request);

            if (affectedRow == 0)
            {
                return BadRequest(); // response lỗi 400 ko tìm thấy 
            }
            return Ok(); // trả về response code là 201
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {

            var affectedRow = await _manageProductService.Delete(productId);

            if (affectedRow == 0)
            {
                return BadRequest(); // response lỗi 400 ko tìm thấy 
            }
            return Ok(); // trả về response code là 201
        }

    }
}
