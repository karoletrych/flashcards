using Flashcards.Infrastructure.HttpClient;
using Flashcards.Models;
using Flashcards.Services.Http;
using Xunit;

namespace Flashcards.ServicesTests
{
    public class ImageBrowserTests
    {
        [Fact]
        public void Test()
        {
            var imageUrlsProvider = new PixabayImageBrowser();

            var urls = imageUrlsProvider.FindAsync("pies", Language.Polish).Result;
            Assert.Equal(9, urls.Count);
        }
    }
}
