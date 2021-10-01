using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.ViewModels.Catalog.Products
{
    public class ProductImageViewModel
    {
        public int Id { get; set; }
        public String FilePath { get; set; }
        public bool IsDefailt { get; set; }
        public long FileSize { get; set; }
    }
}
