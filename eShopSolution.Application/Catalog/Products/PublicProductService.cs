using eShopSolution.Application.Catalog.Products.Dtos;
using eShopSolution.Application.Catalog.Products;
using eShopSolution.Data.EF;
using eShopSolution.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using eShopSolution.ViewModels.Catalog.Products;
using eShopSolution.ViewModels.Catalog.Common;

namespace eShopSolution.Application.Catalog.Products.Impl
{
    public class PublicProductService : IPublicProductService
    {
        private readonly EShopDbContext _context;

        public PublicProductService(EShopDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductViewModel>> GetAll()
        {
            var query = from p in _context.Products
                        join pt in _context.ProductTranslations on p.Id equals pt.Id
                        join pic in _context.ProductInCategories on pt.Id equals pic.ProductId
                        join c in _context.Categories on pic.CategoryId equals c.Id
                        select new { p, pt, pic };

            var data = await query.Select(x => new ProductViewModel()
            {
                Id = x.p.Id,
                Name = x.pt.Name,
                DateCreated = x.p.DateCreated,
                Description = x.pt.Description,
                Details = x.pt.Details,
                LanguageId = x.pt.LanguageId,
                OriginalPrice = x.p.OriginalPrice,
                Price = x.p.Price,
                SeoAlias = x.pt.SeoAlias,
                SeoTitle = x.pt.SeoTitle,
                SeoDescription = x.pt.SeoDescription,
                ViewCount = x.p.ViewCount,
                Stock = x.p.Stock
            }).ToListAsync();

            return data;
        }

        public async Task<PageResult<ProductViewModel>> GetAllByCategoryId(GetPublicProductPagingRequest request)
        {
            // 1.select join
            var query = from p in _context.Products
                        join pt in _context.ProductTranslations on p.Id equals pt.Id
                        join pic in _context.ProductInCategories on pt.Id equals pic.ProductId
                        join c in _context.Categories on pic.CategoryId equals c.Id
                        select new { p, pt, pic };

            /* 2.filter
                + Kiếm tra nếu có từ khóa keyword được search
                + kiểm tra nếu search theo list categoryID
             
             */

            //Nếu request yêu cầu có thêm tìm kiếm theo danh mục
            if (request.CategoryId > 0)
            {
                query = query.Where(x => x.pic.CategoryId == request.CategoryId);
            }


            // 3. paging

            //1. Lấy số record trả về
            int totalRow = await query.CountAsync();

            /*2. Lấy list product theo phân trang
                + Skip : là bỏ qua các ptu 
                + Take: lấy các ptu

                Ex: 
                    + Ví dụ pageIndex = 1 , pageSize = 20
                    Skip =  0 
                    Take = 20

                    ==> sẽ trả về 20 record đầu tiên

                    + Ví dụ pageIndex = 2 , pageSize = 20
                    Skip =  20 
                    Take = 20

                    ==> sẽ trả về 20 record tiếp theo
             */


            var data = await query.Skip((request.pageIndex - 1) * request.pageSize)
                .Take(request.pageSize)
                .Select(x => new ProductViewModel
                {
                    Id = x.p.Id,
                    Name = x.pt.Name,
                    DateCreated = x.p.DateCreated,
                    Description = x.pt.Description,
                    Details = x.pt.Details,
                    LanguageId = x.pt.LanguageId,
                    OriginalPrice = x.p.OriginalPrice,
                    Price = x.p.Price,
                    SeoAlias = x.pt.SeoAlias,
                    SeoTitle = x.pt.SeoTitle,
                    SeoDescription = x.pt.SeoDescription,
                    ViewCount = x.p.ViewCount,
                    Stock = x.p.Stock
                }).ToListAsync();


            var pageResult = new PageResult<ProductViewModel>()
            {
                totalRecord = totalRow,
                Items = data
            };


            return pageResult;
        }
    }
}
