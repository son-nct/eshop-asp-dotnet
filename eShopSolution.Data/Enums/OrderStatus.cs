using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Data.Enums
{
    // enum là dùng để khai báo các hằng thành phần có kiểu dữ liệu là number
    // bắt đầu từ 0
    public enum OrderStatus
    {
        InProgress, // 0
        Confirmed,  // 1
        Shipping,   // 2
        Success,    // 3
        Canceled    // 4
    }
}
