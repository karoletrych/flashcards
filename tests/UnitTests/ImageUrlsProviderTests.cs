using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlashCards.Models;
using FlashCards.Models.Dto;
using Xunit;

namespace FlashCards.UnitTests
{
    public class ImageUrlsProviderTests
    {
        [Fact]
        public void Test()
        {
            var imageUrlsProvider = new ImageUrlsProvider(9);

            var urls = imageUrlsProvider.Find("pies", Language.Polish).Result;
            Assert.Equal(9, urls.Count());
        }
    }
}
