using eShopSolution.Application.Catalog.Products.Dtos;
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
        * Vào startup.css phần ConfigureServices
        * Declare DI
        * Chỉ dịnh khi khai báo DI Interface nó sẽ hiểu là tiêm thằng class impl interface đó
        * 
        * services.AddTranient(IPublicProductService,PublicProductService>();
        * AddTransient(thoáng qua) : có nghĩa là khi có req thì sẽ inject vào
        * 
        */

        private readonly IPublicProductService _publicProductService;

        public ProductController(IPublicProductService publicProductService)
        {
            _publicProductService = publicProductService;
        }

        [HttpGet]
        public async Task<IActionResult> Get() {

            var products = await _publicProductService.GetAll();
            return Ok(products);
        }
    }
}
