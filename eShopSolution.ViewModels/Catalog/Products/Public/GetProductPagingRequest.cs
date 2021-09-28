using eShopSolution.ViewModels.Catalog.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.ViewModels.Catalog.Products.Public
{
    public class GetProductPagingRequest : PageingRequestBase
    {
        public int CategoryId { get; set; }
    }
}
