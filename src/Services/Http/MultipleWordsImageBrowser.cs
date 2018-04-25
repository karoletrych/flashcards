using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flashcards.Models;
using Flashcards.Services.Examiner;

namespace Flashcards.Services.Http
{
	public class MultipleWordsImageBrowser : IImageBrowser
	{
		private readonly IImageBrowser _internalImageBroser;

		public MultipleWordsImageBrowser(IImageBrowser internalImageBroser)
		{
			_internalImageBroser = internalImageBroser;
		}

		public async Task<IList<Uri>> Find(string query, Language queryLanguage, int numberOfResults)
		{
			var images = await _internalImageBroser.Find(query, queryLanguage, numberOfResults);
			if (images.Count >= numberOfResults)
				return images;

			var wordImagesLists = 
				await Task.WhenAll(
					query
					.Split(' ')
					.Select(word => _internalImageBroser.Find(word, queryLanguage, numberOfResults)));

			var wordImages = wordImagesLists.SelectMany(x=>x.AsEnumerable());
			return images
				.Concat(wordImages)
				.Shuffle()
				.Take(numberOfResults)
				.ToList();
		}
	}
}