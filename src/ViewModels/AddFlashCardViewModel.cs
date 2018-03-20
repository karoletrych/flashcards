using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
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
		private Language _frontLanguage;
		private Language _backLanguage;
		private readonly ITranslator _translator;
		private string _lessonId;

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

		public Uri SelectedImageUri { get; set; }

		public string FrontText { get; set; }

		public string BackText { get; set; }

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
			parameters.Add("lessonId", _lessonId);
		}

		public event PropertyChangedEventHandler PropertyChanged;


		private async Task UpdateImages(Language language, string text)
		{
			var imageUris = await _imageBrowser.Find(text, language);
			ImageUris.Clear();
			foreach (var imageUri in imageUris) ImageUris.Add(imageUri);
		}

		public async Task HandleFrontTextChangedByUser()
		{
			if(!string.IsNullOrEmpty(BackText))
				return;

			async Task UpdateTranslation()
			{
				var translations = await _translator.Translate(
					from: _frontLanguage,
					to: _backLanguage,
					text: FrontText);
				BackText = string.Join("", translations);
			}

			var updateImages = UpdateImages(_frontLanguage, FrontText);

			await Task.WhenAll(UpdateTranslation(), updateImages);
		}

		public async Task HandleBackTextChangedByUser()
		{
			if(!string.IsNullOrEmpty(FrontText))
				return;

			async Task UpdateTranslation()
			{
				var translations = await _translator.Translate(
					from: _backLanguage,
					to: _frontLanguage,
					text: BackText);
				FrontText = string.Join("", translations);
			}
			var updateImages = UpdateImages(_backLanguage, BackText);

			await Task.WhenAll(UpdateTranslation(), updateImages);
		}
	}
}