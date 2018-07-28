using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flashcards.Domain.ViewModels;
using Flashcards.Models;
using Flashcards.SpacedRepetition.Interface;
using FluentAssertions;
using NSubstitute;
using Prism.Navigation;
using Xunit;

namespace ViewModelsTests
{
	public class FlashcardListViewModelTests
	{
		private readonly FlashcardListViewModel _sut;
		private readonly Lesson _lesson = Lesson.Create(Language.English, Language.Polish, new List<Flashcard>
		{
			Flashcard.Create("cat", "kot"),
			Flashcard.Create("dog", "pies"),
			Flashcard.Create("duck", "kaczka")
		});

		public FlashcardListViewModelTests()
		{
			var navigationService = Substitute.For<INavigationService>();
			var repository = TestRepository.InitializedWith(_lesson.Flashcards);
			var getFlashcardsKnowledgeLevels = Substitute.For<IGetFlashcardsKnowledgeLevels>();

			_sut = new FlashcardListViewModel(repository, navigationService, getFlashcardsKnowledgeLevels);

			_sut.OnNavigatedTo(new NavigationParameters
			{
				{"lesson", _lesson}
			});
		}

		[Fact]
		public void WhenCreationDateButtonTapped_FlashcardListIsNotEmpty()
		{
			_sut.SortByCreationDate.Execute(null);
			_sut.Flashcards.Should().NotBeEmpty();
		}

		[Fact]
		public void WhenCreationDateButtonTapped_FlashcardListIsSortedByCreationDate()
		{
			_sut.SortByCreationDate.Execute(null);

			_sut.Flashcards.Should().BeInAscendingOrder(f => f.Created);
		}

		[Fact]
		public void WhenCreationDateButtonTappedTwice_FlashcardListIsSortedByCreationDateDescending()
		{
			_sut.SortByCreationDate.Execute(null);
			_sut.SortByCreationDate.Execute(null);

			_sut.Flashcards.Should().BeInDescendingOrder(f => f.Created);
		}

		[Fact]
		public void WhenFrontButtonTapped_FlashcardListIsSortedByFrontText()
		{
			_sut.SortByFront.Execute(null);

			_sut.Flashcards.Should().BeInAscendingOrder(f => f.Front);
		}

		[Fact]
		public void WhenFrontButtonTappedTwice_FlashcardListIsSortedByFrontTextDescending()
		{
			_sut.SortByFront.Execute(null);
			_sut.SortByFront.Execute(null);

			_sut.Flashcards.Should().BeInDescendingOrder(f => f.Front);
		}

		[Fact]
		public void WhenBackButtonTapped_FlashcardListIsSortedByBackText()
		{
			_sut.SortByBack.Execute(null);

			_sut.Flashcards.Should().BeInAscendingOrder(f => f.Back);
		}

		[Fact]
		public void WhenBackButtonTappedTwice_FlashcardListIsSortedByBackTextDescending()
		{
			_sut.SortByBack.Execute(null);
			_sut.SortByBack.Execute(null);

			_sut.Flashcards.Should().BeInDescendingOrder(f => f.Back);
		}
	}
}