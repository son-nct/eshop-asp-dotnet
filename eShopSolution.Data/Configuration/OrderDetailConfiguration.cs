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
    public class OrderDetailConfiguration : IEntityTypeConfiguration<OrderDetail>
    {
        public void Configure(EntityTypeBuilder<OrderDetail> builder)
        {
            builder.HasKey(x => new { x.OrderId, x.ProductId });
            builder.HasOne(o => o.Order).WithMany(o => o.OrderDetails).HasForeignKey(od => od.OrderId);
            builder.HasOne(p => p.Product).WithMany(p => p.OrderDetails).HasForeignKey(od => od.ProductId);
        }
    }
}
