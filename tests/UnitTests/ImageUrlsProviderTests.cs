﻿using System.Linq;
using Flashcards.Models;
using Flashcards.Services;
using Flashcards.Services.Http;
using Xunit;

namespace Flashcards.UnitTests
{
    public class ImageUrlsProviderTests
    {
        [Fact]
        public void Test()
        {
            var imageUrlsProvider = new PixabayImageUrlsProvider(9);

            var urls = imageUrlsProvider.Find("pies", Language.Polish).Result;
            Assert.Equal(9, urls.Count());
        }
    }
}
