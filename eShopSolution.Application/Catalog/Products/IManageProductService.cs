using eShopSolution.ViewModels.Catalog.Common;
using eShopSolution.ViewModels.Catalog.ProductImages;
using eShopSolution.ViewModels.Catalog.Products;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.Catalog.Products
{
    //interface này cho admin
    public interface IManageProductService
    {
        /*
         * tất cả các abstract method nên dùng thread async để xử lí bất đồng bộ giúp tiết kiệm thời gian
         * Thay vì phải đợi 1 thread làm xong nhiệm vụ thì ta có thể thực hiện nhiều task khác để tiết kiệm
            tài nguyên và thời gian

            Syntax:
                        Task<kieu_tra_ve> method();

            Ex:   Task<int> Create(ProductCreateRequest request);
         */

        // các abstract method interface ko cần khai báo kiểu access modifier
        Task<int> Create(ProductCreateRequest request);

        Task<int> Update(ProductUpdateRequest request);

        Task<int> Delete(int productId);

        Task<bool> UpdatePrice(int productId, decimal newPrice);

        Task AddViewCount (int productId); // mỗi lần tăng count lên 1

        Task<bool> UpdateStock(int productId, int addedQuantity);

        Task<ProductViewModel> GetById(int productId, string languageId);

        /*
             lấy view chung từ DTOS(hiển thị thông tin phân trang trả về List mình định nghĩa)
             + GetProductPagingRequest là 1 class extend PagingRequestBase chứa pageIndex và pageSize
             + GetProductPagingRequest:
                + sẽ định nghĩa thêm keyword ==> cách này giúp code lean hơn
                + và 1 list categoryId
        */

        Task<PageResult<ProductViewModel>> GetAllPaging(GetManageProductPagingRequest request);


        Task<int> AddImage(int productId, ProductImageCreateRequest productImage);

        Task<int> RemoveImage(int productId, int imageId);

        Task<int> UpdateImage(int imageId, ProductImageUpdateRequest productImage);

        Task<List<ProductImageViewModel>> GetListImages(int productId);

    }
}
