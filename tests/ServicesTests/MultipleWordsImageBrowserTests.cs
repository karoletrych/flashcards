using System;
using System.Collections.Generic;
using System.Linq;
using Flashcards.Models;
using Flashcards.Services.Http;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Flashcards.ServicesTests
{
	public class MultipleWordsImageBrowserTests
	{
		private readonly MultipleWordsImageBrowser _sut;
		private readonly IImageBrowser _internalImageBrowser;

		public MultipleWordsImageBrowserTests()
		{
			_internalImageBrowser = Substitute.For<IImageBrowser>();
			_sut = new MultipleWordsImageBrowser(_internalImageBrowser);
		}

		private readonly List<Uri> _uris = new List<Uri>
		{
			new Uri("http://0"),
			new Uri("http://1"),
			new Uri("http://2"),
			new Uri("http://3"),
			new Uri("http://4"),
			new Uri("http://5"),
		};

		[Fact]
		public async void when_internal_image_browser_returns_less_images_than_requested__retries_querying_each_word()
		{
			var phrase = "the quick brown fox";
			var numberOfResults = 20;

			_internalImageBrowser.Find(phrase, Language.English, numberOfResults).Returns(new List<Uri>
			{
				_uris[0],
				_uris[1],
				_uris[2],
			});

			await _sut.Find(phrase, Language.English, numberOfResults);

			await _internalImageBrowser.Received().Find("the", Language.English, numberOfResults);
			await _internalImageBrowser.Received().Find("quick", Language.English, numberOfResults);
			await _internalImageBrowser.Received().Find("brown", Language.English, numberOfResults);
			await _internalImageBrowser.Received().Find("fox", Language.English, numberOfResults);
		}

		[Fact]
		public async void returns_all_subresults()
		{
			var phrase = "the quick brown fox";
			var numberOfResults = 20;

			SetupInternalImageBrowser(phrase, numberOfResults);
			var results = await _sut.Find(phrase, Language.English, numberOfResults);

			results.Should().BeEquivalentTo(_uris);
		}

		[Fact]
		public async void limits_number_of_results_to_given_number()
		{
			var phrase = "the quick brown fox";
			var numberOfResults = 5;

			SetupInternalImageBrowser(phrase, numberOfResults);
			var results = await _sut.Find(phrase, Language.English, numberOfResults);

			results.Count.Should().Be(numberOfResults);
		}

		private void SetupInternalImageBrowser(string phrase, int numberOfResults)
		{
			_internalImageBrowser.Find(phrase, Language.English, numberOfResults).Returns(new List<Uri>
			{
				_uris[0],
				_uris[1],
			});
			_internalImageBrowser.Find("the", Language.English, numberOfResults)
				.Returns(new List<Uri>
				{
					_uris[2],
				});
			_internalImageBrowser.Find("quick", Language.English, numberOfResults)
				.Returns(new List<Uri>
				{
					_uris[3],
				});
			_internalImageBrowser.Find("brown", Language.English, numberOfResults)
				.Returns(new List<Uri>
				{
					_uris[4],
				});
			_internalImageBrowser.Find("fox", Language.English, numberOfResults)
				.Returns(new List<Uri>
				{
					_uris[5],
				});
		}
	}
}
