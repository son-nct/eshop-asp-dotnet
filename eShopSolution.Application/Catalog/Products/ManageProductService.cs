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
using eShopSolution.ViewModels.Catalog.Products;
using eShopSolution.ViewModels.Catalog.Common;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.IO;
using eShopSolution.Application.Common;
using eShopSolution.ViewModels.Catalog.ProductImages;

namespace eShopSolution.Application.Catalog.Products
{
    public class ManageProductService : IManageProductService
    {
        // Thực hiện dependency injection EShopSolution.Data vào phần dependencies
        // inject EShopDbContext thông qua constructor injection

        private readonly EShopDbContext _context;

        //inject thằng storage service để thao tác vs file
        private readonly IStorageService _storageService;


        public ManageProductService(EShopDbContext context, IStorageService storageService)
        {
            _context = context;
            _storageService = storageService;
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
                        SeoTitle = request.SeoTitle,
                        LanguageId = request.LanguageId
                    }
                }

            };


            //SaveImg
            if (request.ThumbnailImage != null)
            {
                product.ProductImages = new List<ProductImage>()
                {
                   new ProductImage()
                   {
                       Caption = "Thumbnail image",
                       DateCreated = DateTime.Now,
                       FileSize = request.ThumbnailImage.Length,
                       ImagePath = await this.SaveFile(request.ThumbnailImage),
                       IsDefault = true,
                       SortOrder = 1
                   }
                };
            }


            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product.Id;
        }

        public async Task<int> Update(ProductUpdateRequest request)
        {
            var product = await _context.Products.FindAsync(request.Id);
            var productTranslation = await _context.ProductTranslations.Where(x => x.ProductId == request.Id && x.LanguageId == request.LanguageId).FirstOrDefaultAsync();


            // chỉ cập nhật lại thằng productTranslation chứa language id cụ thể
            if (product == null && productTranslation == null)
            {
                throw new EShopException($"Cannot find a product : {request.Id}");

            }

            productTranslation.Name = request.Name;
            productTranslation.Details = request.Details;
            productTranslation.Description = request.Description;
            productTranslation.SeoAlias = request.SeoAlias;
            productTranslation.SeoTitle = request.SeoTitle;
            productTranslation.SeoDescription = request.SeoDescription;

            //nếu có yêu cầu update
            if (request.ThumbnailImage != null)
            {
                //kiểm tra thumbnail có tồn tại ko
                var thumbnailImage = await _context.ProductImages.FirstOrDefaultAsync(x => x.IsDefault == true && x.ProductId == request.Id);
                if (thumbnailImage != null)
                {
                    thumbnailImage.FileSize = request.ThumbnailImage.Length;
                    thumbnailImage.ImagePath = await this.SaveFile(request.ThumbnailImage);

                    _context.ProductImages.Update(thumbnailImage);
                }
            }

            return await _context.SaveChangesAsync();

        }

        public async Task<int> Delete(int productId)
        {
            var product = await _context.Products.FindAsync(productId);

            if (product == null)
            {
                throw new EShopException($"Cannot find a product : {productId}");
            }

            // trước khi xóa toàn bộ dưới db kiểm tra còn tồn tại file ảnh nào thì xóa luôn
            var images = _context.ProductImages.Where(x => x.ProductId == productId);
            foreach (var img in images)
            {
                await _storageService.DeleteFileAsync(img.ImagePath);
            }

            _context.Products.Remove(product);
            return await _context.SaveChangesAsync();
        }

        public async Task<PageResult<ProductViewModel>> GetAllPaging(GetManageProductPagingRequest request)
        {

            // 1.select join
            var query = from p in _context.Products
                        join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                        join pic in _context.ProductInCategories on p.Id equals pic.ProductId
                        join c in _context.Categories on pic.CategoryId equals c.Id
                        select new { p, pt, pic };

            /* 2.filter
                + Kiếm tra nếu có từ khóa keyword được search
                + kiểm tra nếu search theo list categoryID
             
             */
            if (!String.IsNullOrEmpty(request.keyword))
            {
                // x ở đây đc hiểu là 1 list select new {p , pt , pic}
                query = query.Where(x => x.pt.Name.Contains(request.keyword));
            }

            //Nếu request yêu cầu có thêm tìm kiếm theo danh mục
            if (request.CategoryIds.Count > 0)
            {
                query = query.Where(x => request.CategoryIds.Contains(x.pic.CategoryId));
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

        public async Task<ProductViewModel> GetById(int productId, string languageId)
        {
            var product = await _context.Products.FindAsync(productId);



            var productTranslation = await _context.ProductTranslations.FirstOrDefaultAsync(x => x.ProductId == productId
                                            && x.LanguageId == languageId);

            if (product == null && productTranslation == null)
            {
                throw new EShopException($"Not found product {productId}");
            }

            
            var categories = await (from c in _context.Categories
                                    join ct in _context.CategoryTranslations on c.Id equals ct.CategoryId
                                    join pic in _context.ProductInCategories on ct.CategoryId equals pic.CategoryId
                                    where pic.ProductId == productId && ct.LanguageId == languageId
                                    select ct.Name).ToListAsync();

            var image = await _context.ProductImages.FirstOrDefaultAsync(x => x.ProductId == productId && x.IsDefault == true);

            var productViewModel = new ProductViewModel()
            {
                Id = product.Id,
                DateCreated = product.DateCreated,
                Description = productTranslation != null ? productTranslation.Description : null,
                LanguageId = productTranslation.LanguageId,
                Details = productTranslation != null ? productTranslation.Details : null,
                Name = productTranslation != null ? productTranslation.Name : null,
                OriginalPrice = product.OriginalPrice,
                Price = product.Price,
                SeoAlias = productTranslation != null ? productTranslation.SeoAlias : null,
                SeoTitle = productTranslation != null ? productTranslation.SeoTitle : null,
                SeoDescription = productTranslation != null ? productTranslation.SeoDescription : null,
                Stock = product.Stock,
                ViewCount = product.ViewCount,
                Categories = categories,
                ThumbnailImage = image != null ? image.ImagePath : "no-image.jpg"
            };

            return productViewModel;

        }


        public async Task<bool> UpdatePrice(int productId, decimal newPrice)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                throw new EShopException($"Cannot find a product : {productId}");
            }

            product.Price = newPrice;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateStock(int productId, int addedQuantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                throw new EShopException($"Cannot find a product : {productId}");
            }

            product.Stock = addedQuantity;
            return await _context.SaveChangesAsync() > 0;
        }

        //này dùng để upload file 
        // trả về đường dẫn của file đó
        private async Task<String> SaveFile(IFormFile file)
        {

            //ContentDispositionHeaderValue working with fileUpload and download

            //lấy tên fileUload
            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');

            //add guid vào tên file để tránh bị trùng
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";

            //sau đó lưu file vào đường dẫn root
            await _storageService.SaveFileAsync(file.OpenReadStream(), fileName);
            return fileName;
        }

        public async Task<int> AddImage(int productId, ProductImageCreateRequest request)
        {

            var productImage = new ProductImage()
            {
                Caption = request.Caption,
                DateCreated = DateTime.Now,
                IsDefault = request.IsDefault,
                ProductId = productId,
                SortOrder = request.SortOrder
            };

            if (request.ImageFile != null)
            {
                productImage.ImagePath = await this.SaveFile(request.ImageFile);
                productImage.FileSize = request.ImageFile.Length;
            }

            _context.ProductImages.Add(productImage);
             await _context.SaveChangesAsync();
            return productImage.Id;
        }

        public async Task<int> RemoveImage( int imageId)
        {

            var productImage = await _context.ProductImages.FindAsync(imageId);
            if (productImage == null)
            {
                throw new EShopException($"Not found image {imageId}");
            }
            _context.ProductImages.Remove(productImage);

            return await _context.SaveChangesAsync();

        }

        public async Task<int> UpdateImage(int imageId, ProductImageUpdateRequest request)
        {
            var productImage = await _context.ProductImages.FindAsync(imageId);

            if (productImage == null)
            {
                throw new EShopException($"Not found image {imageId}");
            }
            else
            {
                productImage.ImagePath = await this.SaveFile(request.ImageFile);
                productImage.FileSize = request.ImageFile.Length;
            }

            _context.ProductImages.Update(productImage);
            return await _context.SaveChangesAsync();

        }

        public async Task<List<ProductImageViewModel>> GetListImages(int productId)
        {

            return await _context.ProductImages.Where(x => x.ProductId == productId)
                .Select(x => new ProductImageViewModel()
                {
                    Id = x.Id,
                    Caption = x.Caption,
                    DateCreated = x.DateCreated,
                    FileSize = x.FileSize,
                    ImagePath = x.ImagePath,
                    IsDefault = x.IsDefault,
                    ProductId = x.ProductId,
                    SortOrder = x.SortOrder
                }).ToListAsync();
        }

        public async Task<ProductImageViewModel> GetImageById(int imageId)
        {
            var image = await _context.ProductImages.FindAsync(imageId);

            if(image == null)
            {
                throw new EShopException($"Not found image {imageId}");
            }

            var imageViewModel = new ProductImageViewModel()
            {
                Caption = image.Caption,
                DateCreated = image.DateCreated,
                FileSize = image.FileSize,
                ImagePath = image.ImagePath,
                Id = image.Id,
                IsDefault = image.IsDefault,
                ProductId = image.ProductId,
                SortOrder = image.SortOrder,
            };

            return imageViewModel;
        }
    }
}
