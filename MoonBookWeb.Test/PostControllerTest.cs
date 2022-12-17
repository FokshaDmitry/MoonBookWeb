using MoonBookWeb.API;
using MoonBookWeb.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MoonBookWeb.Midelewere;
using Xunit;
using Microsoft.AspNetCore.Http;

namespace MoonBookWeb.Test
{
    public class PostControllerTest
    {
        [Fact]
        public void TestGet()
        {
            DbContextOptionsBuilder<AddDbContext> optionsBuilder = new DbContextOptionsBuilder<AddDbContext>();
            var options = optionsBuilder.UseSqlServer("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=D:\\Проекты\\MoonBookWeb\\MoonBookWeb\\DbaseMB.mdf;Integrated Security=True").Options; 
            AddDbContext addDbContext = new AddDbContext(options);
            
            ISessionLogin sessionLogin = new SessionLoginServices(addDbContext);
            sessionLogin.Set("a02911b7-317e-48c9-9eb4-08da6a40fc3e");
            PostController postController = new PostController(sessionLogin, addDbContext);

            var result =  postController.Get("user");
            Assert.NotNull(result);
        }
    }
}
