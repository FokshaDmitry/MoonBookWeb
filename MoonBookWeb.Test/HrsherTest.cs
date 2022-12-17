using MoonBookWeb.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MoonBookWeb.Test
{
    public class HrsherTest
    {
        [Fact]
        public void HTest()
        {
            Hesher hesher = new Hesher();

            var result = hesher.Hesh("Some");

            Assert.NotNull(result);
            String resultString = "";
            Assert.IsType(resultString.GetType(), result);
        }
    }
}
