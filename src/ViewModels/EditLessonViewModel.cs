using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Flashcards.Models;
using Flashcards.Services.DataAccess;
using Prism.Navigation;
using Xamarin.Forms;

namespace Flashcards.ViewModels
{
	public class EditLessonViewModel : INavigatedAware, INotifyPropertyChanged
	{
		private readonly IRepository<Lesson> _lessonRepository;
		private readonly INavigationService _navigationService;
		private readonly IRepository<Flashcard> _flashcardRepository;

		private Lesson _lesson;
		public string LessonName { get; set; }

		public EditLessonViewModel()
		{
		}

		public EditLessonViewModel(INavigationService navigationService, IRepository<Lesson> lessonRepository, IRepository<Flashcard> flashcardRepository)
		{
			_navigationService = navigationService;
			_lessonRepository = lessonRepository;
			_flashcardRepository = flashcardRepository;
		}

		public ObservableCollection<Flashcard> Flashcards { get; set; } =
			new ObservableCollection<Flashcard>();

		public ICommand AddFlashcardsCommand => new Command(() =>
			_navigationService.NavigateAsync("AddFlashcardPage", new NavigationParameters
			{
				{
					"frontLanguage", _lesson.FrontLanguage
				},
				{
					"backLanguage", _lesson.BackLanguage
				},
				{
					"lessonId", _lesson.Id
				}
			}));

		public string AskingMode { get; set; }
		public IList<string> AllAskingModes => Enum.GetNames(typeof(AskingMode));

		public bool AskInRepetitions { get; set; }

		public ICommand DeleteFlashcardCommand => new Command<int>(async flashcardId =>
		{
			var flashcardToRemove = new Flashcard{Id = flashcardId};
			await _flashcardRepository.Delete(flashcardToRemove);
			Flashcards.Remove(flashcardToRemove);
		});

		public async void OnNavigatedTo(NavigationParameters parameters)
		{
			var lessonId = (string)parameters["lessonId"];

			_lesson = (await _lessonRepository.FindMatching(lesson => lesson.Id == lessonId)).Single();

			AskInRepetitions = _lesson.AskInRepetitions;
			AskingMode = _lesson.AskingMode.ToString();
			LessonName = _lesson.Name;

			Flashcards.Clear();
			foreach (var flashcard in _lesson.Flashcards) Flashcards.Add(flashcard);

		}

		public void OnNavigatedFrom(NavigationParameters parameters)
		{
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}