using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace eShopSolution.Data.EF
{
    public class EShopDbContextFactory : IDesignTimeDbContextFactory<EShopDbContext>
    {
        public EShopDbContext CreateDbContext(string[] args)
        {
            //Ctrl + k + c để comment block code
            // Ctrl + k + u để gỡ cmt block code

            //gắn file json appsetting vào thư mục context factory hiện tại
            //directory: danh mục
            IConfigurationRoot configurationRoot = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // đặt đường dẫn cơ sở là thư mục hiện tại
                .AddJsonFile("appsetting.json") // add file json setting vào để connection string
                .Build();



            //tạo connectionstring
            var connectionString = configurationRoot.GetConnectionString("eShopSolutionDb");

            // kết nối sql server
            var optionsBuilder = new DbContextOptionsBuilder<EShopDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new EShopDbContext(optionsBuilder.Options);
        }
    }
}
