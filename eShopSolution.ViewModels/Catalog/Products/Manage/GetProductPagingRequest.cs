using eShopSolution.ViewModels.Catalog.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.ViewModels.Catalog.Products.Manage
{
    // class này sẽ extend class PageingRequestBase để thừa kế lại thuộc tính pageIndex và pageSize
    public class GetProductPagingRequest : PageingRequestBase
    {

        //định nghĩa thêm trường keyword
        public string keyword { get; set; }

        public List<int> CategoryIds { get; set; }
    }
}
