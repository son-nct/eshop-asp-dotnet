using eShopSolution.Application.Catalog.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.Catalog.Products.Dtos.Public
{
    public class GetProductPagingRequest: PageingRequestBase
    {
        public int CategoryId { get; set; }
    }
}
