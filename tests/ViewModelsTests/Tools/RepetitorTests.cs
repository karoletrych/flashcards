﻿using System.Collections.Generic;
using System.Linq;
using Flashcards.Domain.ViewModels.Tools;
using Flashcards.Models;
using Flashcards.Services;
using Flashcards.Services.Examiner.Builder;
using Flashcards.SpacedRepetition.Interface;
using NSubstitute;
using Prism.Navigation;
using Xunit;

namespace ViewModelsTests.Tools
{
	public class RepetitorTests
	{
		public RepetitorTests()
		{
			_spacedRepetition = Substitute.For<ISpacedRepetition>();
			_sut = new Repetitor(_spacedRepetition);
		}

		[Fact]
		public async void HandlerIsUnsubscribedFromSessionEndedEvent_WhenExaminerIsDisposed()
		{
			var navigationService = Substitute.For<INavigationService>();
			var flashcard = new Flashcard();
			var examiner = new ExaminerBuilder()
				.WithFlashcards(new[]
				{
					new FlashcardsInLanguage(Language.English, Language.Polish, new List<Flashcard>
					{
						flashcard
					})
				})
				.Build();

			var uri = "AskingQuestionsPage";
			await _sut.Repeat(navigationService, uri, examiner);

			examiner.Dispose();

			examiner.TryAskNextQuestion(out _);
			examiner.Answer(false);
			await _spacedRepetition
				.Received(0)
				.SubmitRepetitionResults(Arg.Any<IEnumerable<QuestionResult>>());
		}

		[Fact]
		public async void NavigatesToAskingQuestionsPageWithExaminer()
		{
			var navigationService = Substitute.For<INavigationService>();
			var examiner = new ExaminerBuilder()
				.WithFlashcards(new[]
				{
					new FlashcardsInLanguage(Language.English, Language.Polish, new List<Flashcard>
					{
						new Flashcard()
					})
				})
				.Build();

			var uri = "AskingQuestionsPage";
			await _sut.Repeat(navigationService, uri, examiner);
			await navigationService.Received()
				.NavigateAsync(uri, Arg.Is<NavigationParameters>(p => p["examiner"] == examiner));
		}

		[Fact]
		public async void RepetitionResultsAreSubmittedOnlyOnce_AfterSessionEndedIsRaised()
		{
			var navigationService = Substitute.For<INavigationService>();
			var flashcard = new Flashcard();
			var examiner = new ExaminerBuilder()
				.WithFlashcards(new[]
				{
					new FlashcardsInLanguage(Language.English, Language.Polish, new List<Flashcard>
					{
						flashcard
					})
				})
				.Build();

			var uri = "AskingQuestionsPage";
			await _sut.Repeat(navigationService, uri, examiner);

			examiner.TryAskNextQuestion(out _);
			examiner.Answer(false);
			examiner.TryAskNextQuestion(out _);
			examiner.Answer(true);

			await _spacedRepetition
				.Received(1)
				.SubmitRepetitionResults(Arg.Is<IEnumerable<QuestionResult>>(qr => qr.Single().Flashcard == flashcard));
		}

		private readonly Repetitor _sut;
		private readonly ISpacedRepetition _spacedRepetition;
	}
}