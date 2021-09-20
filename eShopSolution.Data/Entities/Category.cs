using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Data.Entities
{
    public class Category
    {
        public int Id { get; set; }

        public int SortOrder { get; set; }

        public int IsShowOnHome { get; set; }

        public int ParentId { get; set; }

        public int Status { get; set; }

        public List<ProductInCategory> ProductInCategories { get; set; }

        public List<CategoryTranslation> CategoryTranslations { get; set; }
    }
}
