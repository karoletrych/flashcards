using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flashcards.Models;
using Flashcards.Services.Http;
using LanguageExtensions;

namespace Flashcards.Infrastructure.HttpClient
{
	public class MultipleWordsImageBrowser : IImageBrowser
	{
		private readonly IImageBrowser _internalImageBroser;

		public MultipleWordsImageBrowser(IImageBrowser internalImageBroser)
		{
			_internalImageBroser = internalImageBroser;
		}

		public async Task<IList<Uri>> FindAsync(string query, Language queryLanguage, int numberOfResults)
		{
			var images = await _internalImageBroser.FindAsync(query, queryLanguage, numberOfResults);
			if (images.Count >= numberOfResults)
				return images;

			var wordImagesLists = 
				await Task.WhenAll(
					query
					.Split(' ')
					.Select(word => _internalImageBroser.FindAsync(word, queryLanguage, numberOfResults)));

			var wordImages = wordImagesLists.SelectMany(x=>x.AsEnumerable());
			return images
				.Concat(wordImages)
				.Shuffle()
				.Take(numberOfResults)
				.ToList();
		}
	}
}