﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Flashcards.Domain.ViewModels.Tools;
using Flashcards.Models;
using Flashcards.Services.DataAccess;
using Prism.Navigation;
using Xamarin.Forms;

namespace Flashcards.Domain.ViewModels
{
	public class FlashcardListViewModel : INotifyPropertyChanged, INavigatedAware
	{
		private readonly IRepository<Flashcard> _flashcardRepository;
		private readonly INavigationService _navigationService;
		private Lesson _lesson;

		public FlashcardListViewModel(
			IRepository<Flashcard> flashcardRepository,
			INavigationService navigationService)
		{
			_flashcardRepository = flashcardRepository;
			_navigationService = navigationService;
		}

		public FlashcardListViewModel()
		{
		}

		public ObservableCollection<Flashcard> Flashcards { get; set; } = new ObservableCollection<Flashcard>();

		public ICommand DeleteFlashcardCommand => new Command<string>(async flashcardId =>
		{
			var flashcardToRemove = new Flashcard {Id = flashcardId};
			await _flashcardRepository.Delete(flashcardToRemove);
			Flashcards.Remove(flashcardToRemove);
		});

		public ICommand AddFlashcardsCommand => new Command(() =>
			_navigationService.NavigateAsync("AddFlashcardPage",
				new NavigationParameters
				{
					{
						"lesson", _lesson
					}
				}));

		public void OnNavigatedFrom(NavigationParameters parameters)
		{
		}

		public async void OnNavigatedTo(NavigationParameters parameters)
		{
			_lesson = (Lesson) parameters["lesson"];
			var flashcards = await _flashcardRepository.GetWithChildren(f => f.LessonId == _lesson.Id);

			Flashcards.SynchronizeWith(flashcards);
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}