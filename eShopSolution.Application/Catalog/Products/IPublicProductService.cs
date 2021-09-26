using eShopSolution.Application.Catalog.Dtos;
using eShopSolution.Application.Catalog.Products.Dtos.Public;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.Catalog.Products.Dtos
{
    public interface IPublicProductService
    {
        Task<PageResult<ProductViewModel>> GetAllPaging(GetProductPagingRequest request);
    }
}
