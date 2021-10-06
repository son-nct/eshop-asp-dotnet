using eShopSolution.Application.Catalog.Products;
using eShopSolution.Application.Catalog.Products.Dtos;
using eShopSolution.ViewModels.Catalog.ProductImages;
using eShopSolution.ViewModels.Catalog.Products;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace eShopSolution.BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
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

        public ProductsController(IPublicProductService publicProductService, IManageProductService manageProductService)
        {
            _publicProductService = publicProductService;
            _manageProductService = manageProductService;

        }


        // http://localhost:port/products?pageIndex=1&pageSize=10&CategoryId=?
        [HttpGet("{languageId}")] // đặt bị danh cho url để ko bị trùng

        /*
         * FromQuery => thông số getPublicproductPagingRequest sẽ đc truyền vào thông qua parameter trên url
        */


        public async Task<IActionResult> GetAllPaging(String languageId, [FromQuery] GetPublicProductPagingRequest request)
        {

            var products = await _publicProductService.GetAllByCategoryId(languageId, request);
            return Ok(products);
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ProductCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var productId = await _manageProductService.Create(request); // trả về productId

            if (productId == 0)
            {
                return BadRequest(); // response lỗi 400 ko tạo dc
            }

            var product = await _manageProductService.GetById(productId, request.LanguageId);
            return CreatedAtAction(nameof(GetById), new { id = productId }, product);
            // trả về response code là 201
            // new {id = productId}  ==> route data to use for generating URL
        }


        // http:localhost:port/product/productId/languageId
        [HttpGet("{productId}/{languageId}")]
        public async Task<IActionResult> GetById(int productId, String languageId)
        {

            var product = await _manageProductService.GetById(productId, languageId);

            if (product == null)
            {
                return BadRequest("Cannot find product"); // response lỗi 400 ko tìm thấy 
            }
            return Ok(product); // trả về response code là 201
        }


        [HttpPut]
        public async Task<IActionResult> Update([FromForm] ProductUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var affectedRow = await _manageProductService.Update(request);

            if (affectedRow == 0)
            {
                return BadRequest(); // response lỗi 400 ko tìm thấy 
            }
            return Ok(); // trả về response code là 201
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> Delete(int productId)
        {

            var affectedRow = await _manageProductService.Delete(productId);

            if (affectedRow == 0)
            {
                return BadRequest(); // response lỗi 400 ko tìm thấy 
            }
            return Ok(); // trả về response code là 201
        }

        // ko xài httpPut khi chỉ update có 1 phần
        // còn update all thì xài httpPut
        [HttpPatch("price/{productId}/{newPrice}")]
        public async Task<IActionResult> updatePrice([FromForm] int productId, decimal newPrice)
        {
            bool isSuccessful = await _manageProductService.UpdatePrice(productId, newPrice);
            if (!isSuccessful)
            {
                return BadRequest();
            }
            return Ok();
        }

        [HttpPost("{productId}/images")]
        public async Task<IActionResult>createImage(int productId,[FromForm] ProductImageCreateRequest request)
        {   
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var imageId = await  _manageProductService.AddImage(productId,request);

            if(imageId == 0)
            {
                return BadRequest();
            }

            var image = _manageProductService.GetImageById(imageId);

            return CreatedAtAction(nameof(getImageById), new { id = imageId }, image);
            //tất cả hàm post nên trả về 201 là Created

            return Ok();
        }
       
        [HttpGet("{productId}/images/{imageId}")]
        public async Task<IActionResult> getImageById(int productId,int imageId)
        {
            var image = await _manageProductService.GetImageById(imageId);

            if(image == null)
            {
                return BadRequest(); 
            }

            return Ok(image);
        }

        [HttpPut("{productId}/images/{imageId}")]
        public async Task<IActionResult> updateImage(int imageId, ProductImageUpdateRequest request)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _manageProductService.UpdateImage(imageId, request);

            if(result == 0)
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpDelete("{productId}/images/{imageId}")]
        public async Task<IActionResult> removeImage(int imageId, ProductImageUpdateRequest request)
        {

            var result = await _manageProductService.RemoveImage(imageId);

            if (result == 0)
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}
