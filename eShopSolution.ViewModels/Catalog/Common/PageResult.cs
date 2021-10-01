using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.ViewModels.Catalog.Common
{

    // class này để dùng chung thể hiện các detail phân trang
    // T ở đây là mọi loại đối tượng khác nhau
    public class PageResult<T>
    {

        //PageResult chứa List Items và tổng số record

        public List<T> Items { get; set; }

        public int totalRecord { get; set; }
    }
}
