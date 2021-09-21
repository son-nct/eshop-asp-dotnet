using eShopSolution.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Data.Configuration
{
    public class ProductInCategoryConfiguration : IEntityTypeConfiguration<ProductInCategory>
    {
        public void Configure(EntityTypeBuilder<ProductInCategory> builder)
        {

            builder.ToTable("ProductInCategories");
            //do bảng này là bảng nhiều nhiều nên ta cần phải configure hai khóa ngoại
            // của Category và Product ==> 2 PK làm 2 khóa ngoại

            builder.HasKey(x => new { x.ProductId, x.CategoryId }); //lấy pk của mỗi bảng làm khóa phú

            //cấu hình hai khóa ngoại

            //1 product sẽ có nhiều product incategory
            builder.HasOne(p => p.Product).WithMany(p => p.ProductInCategories)
                .HasForeignKey(pc => pc.ProductId);

            // 1 category cũng sẽ có nhiều productCategory
            builder.HasOne(c => c.Category).WithMany(c => c.ProductInCategories)
                .HasForeignKey(pc => pc.CategoryId);
        }
    }
}
