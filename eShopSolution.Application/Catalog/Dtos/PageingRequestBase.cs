using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.Catalog.Dtos
{
    public class PageingRequestBase
    {
        // gõ prop rồi tab cho lẹ
        // PagingRequestBase chứa pageIndex và pageSize


        public int pageIndex { get; set; }

        public int pageSize { get; set; }
    }
}
