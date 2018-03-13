using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Timers;
using System.Windows.Input;
using Flashcards.Models;
using Flashcards.Services.DataAccess;
using Flashcards.Services.Http;
using Prism.Navigation;
using Xamarin.Forms;

namespace Flashcards.ViewModels
{
	public class AddFlashcardViewModel : INotifyPropertyChanged, INavigatedAware
	{
		private const int ImagesNumber = 9;
		private readonly IRepository<Flashcard> _flashcardRepository;
		private readonly IImageBrowser _imageBrowser;

		private readonly ITranslator _translator;
		private Language _backLanguage;

		private string _backText;
		private Language _frontLanguage;

		private string _frontText;
		private string _lessonId;
		private Uri _selectedImageUri;

		public AddFlashcardViewModel()
		{
		}

		public AddFlashcardViewModel(
			ITranslator translator,
			IRepository<Flashcard> flashcardRepository,
			IImageBrowser imageBrowser)
		{
			_translator = translator;
			_flashcardRepository = flashcardRepository;
			_imageBrowser = imageBrowser;
		}

		public ObservableCollection<Uri> ImageUris { get; } = new ObservableCollection<Uri>(new Uri[ImagesNumber]);

		public Uri SelectedImageUri
		{
			get => _selectedImageUri;
			set
			{
				if (value == _selectedImageUri) return;
				_selectedImageUri = value;
				OnPropertyChanged();
			}
		}

		public string FrontText
		{
			get => _frontText;
			set
			{
				if (value == _frontText) return;
				_frontText = value;
				OnPropertyChanged();
			}
		}

		public string BackText
		{
			get => _backText;
			set
			{
				if (value == _backText) return;
				_backText = value;
				OnPropertyChanged();
			}
		}

		public ICommand NextFlashcardCommand => new Command(async () =>
		{
			await _flashcardRepository.Insert(new Flashcard
			{
				Front = FrontText,
				Back = BackText,
				LessonId = _lessonId,
				ImageUrl = SelectedImageUri?.AbsoluteUri
			});

			FrontText = "";
			BackText = "";
			ImageUris.Clear();
			SelectedImageUri = null;
		});

		public void OnNavigatedTo(NavigationParameters parameters)
		{
			_frontLanguage = (Language) parameters["frontLanguage"];
			_backLanguage = (Language) parameters["backLanguage"];
			_lessonId = (string) parameters["lessonId"];
		}

		public void OnNavigatedFrom(NavigationParameters parameters)
		{
		}

		public event PropertyChangedEventHandler PropertyChanged;


		public async void HandleFrontTextCompleted()
		{
			var translations = await _translator.Translate(
				from: _frontLanguage,
				to: _backLanguage,
				text: FrontText);
			BackText = string.Join("", translations);

			var imageUris = await _imageBrowser.Find(FrontText, _frontLanguage);
			ImageUris.Clear();
			foreach (var imageUri in imageUris) ImageUris.Add(imageUri);
		}

		public async void HandleBackTextCompleted()
		{
			var translations = await _translator.Translate(
				from: _backLanguage,
				to: _frontLanguage,
				text: BackText);
			FrontText = string.Join("", translations);

			var imageUris = await _imageBrowser.Find(BackText, _backLanguage);
			ImageUris.Clear();
			foreach (var imageUri in imageUris) ImageUris.Add(imageUri);
		}

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}