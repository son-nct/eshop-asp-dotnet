using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.Common
{

    /*interface này dùng để 
     + lấy file
     + save file
     + delete file
     */
    public interface IStorageService
    {
        String GetFileUrl(String filename);

        Task SaveFileAsync(Stream mediaBinaryStream, String fileName);

        Task DeleteFileAsync(String fileName);

    }
}
