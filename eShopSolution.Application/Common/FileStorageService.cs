using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.Common
{
    public class FileStorageService : IStorageService
    {

        private readonly string _userContentFolder;
        private const string USER_CONTENT_FOLDER_NAME = "user-content";


        public FileStorageService(IWebHostEnvironment webHostEnvironment)
        {
            //gắn đường dẫn root vào _userContentFolder
            _userContentFolder = Path.Combine(webHostEnvironment.WebRootPath, USER_CONTENT_FOLDER_NAME);
        }

        public string GetFileUrl(string fileName)
        {
            return $"/{USER_CONTENT_FOLDER_NAME}/{fileName}";

        }

        //SaveFileAsync để lấy đường dẫn hình ảnh

        public async Task SaveFileAsync(Stream mediaBinaryStream, string fileName)
        {

            //combine gắn _usercontentFoleder và fileName thành  1 string ten filePath
            // lấy ra đường dẫn tuyệt đối của fileName
            var filePath = Path.Combine(_userContentFolder, fileName);

            //tạo ra 1 file ở đường dẫn chỉ định
            using var output = new FileStream(filePath, FileMode.Create);

            //sao chép mediabinayStream tới đường dẫn chỉ định là filePath
            await mediaBinaryStream.CopyToAsync(output);

        }

        public async Task DeleteFileAsync(string fileName)
        {
            var filePath = Path.Combine(_userContentFolder, fileName);

            if (File.Exists(filePath))
            {
                //() dấu này là tạo 1 hành động
                await Task.Run(() => File.Delete(filePath));
            }
        }
    }
}
