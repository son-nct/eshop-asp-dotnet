using eShopSolution.Application.Catalog.Dtos;
using eShopSolution.Application.Catalog.Products.Dtos;
using eShopSolution.Application.Catalog.Products.Dtos.Manage;
using eShopSolution.Application.Catalog.Products;
using eShopSolution.Data.EF;
using eShopSolution.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EShopSolution.Utilities.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace eShopSolution.Application.Catalog.Products
{
    public class ManageProductService : IManageProductService
    {
        // Thực hiện dependency injection EShopSolution.Data vào phần dependencies
        // inject EShopDbContext thông qua constructor injection

        private readonly EShopDbContext _context;

        public ManageProductService(EShopDbContext context)
        {
            _context = context;
        }

        public async Task AddViewCount(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            product.ViewCount += 1;
            await _context.SaveChangesAsync();
        }


        public async Task<int> Create(ProductCreateRequest request)
        {
            var product = new Product()
            {
                Price = request.Price,
                OriginalPrice = request.OriginalPrice,
                Stock = request.Stock,
                ViewCount = 0,
                DateCreated = DateTime.Now,
                ProductTranslations = new List<ProductTranslation>()
                {
                    new ProductTranslation() {
                        Name = request.Name,
                        Description = request.Description,
                        Details = request.Details,
                        SeoDescription = request.SeoDescription,
                        SeoAlias = request.SeoAlias,
                        LanguageId = request.LanguageId
                    }
                }
                
            };

            _context.Products.Add(product);
            return await _context.SaveChangesAsync();
            
        }

        public async Task<int> Update(ProductUpdateRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<int> Delete(int productId)
        {
            var product = await _context.Products.FindAsync(productId);

            if(product == null)
            {
                throw new EShopException($"Cannot find a product : {productId}");
            }

            _context.Products.Remove(product);
            return await _context.SaveChangesAsync();
        }

        public async Task<PageResult<ProductViewModel>> GetAllPaging(GetProductPagingRequest request)
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
            if(!String.IsNullOrEmpty(request.keyword))
            {
                // x ở đây đc hiểu là 1 list select new {p , pt , pic}
                query = query.Where(x => x.pt.Name.Contains(request.keyword));
            }
            
            //Nếu request yêu cầu có thêm tìm kiếm theo danh mục
            if(request.CategoryIds.Count > 0)
            {
                query = query.Where(x => request.CategoryIds.Contains(x.pic.CategoryId));
            }


            // 3. paging

            //1. Lấy số record trả về
            int totalRow = await query.CountAsync();

            /*2. Lấy list product theo phân trang
                + Skip : là bỏ qua các ptu 
                + Take: lấy các ptu
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

        public Task<ProductViewModel> GetById(int productId, string languageId)
        {
            throw new NotImplementedException();
        }


        public Task<bool> UpdatePrice(int productId, decimal newPrice)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateStock(int productId, decimal addedQuantity)
        {
            throw new NotImplementedException();
        }

       
    }
}
