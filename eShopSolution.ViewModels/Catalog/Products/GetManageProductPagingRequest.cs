using eShopSolution.ViewModels.Catalog.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.ViewModels.Catalog.Products
{
    public class GetManageProductPagingRequest : PageingRequestBase
    {

        //định nghĩa thêm trường keyword
        public string keyword { get; set; }

        public List<int> CategoryIds { get; set; }
    }
}
