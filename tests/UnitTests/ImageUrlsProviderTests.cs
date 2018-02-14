using System.Linq;
using FlashCards.Models;
using FlashCards.Services;
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
