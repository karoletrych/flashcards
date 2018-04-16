using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using Flashcards.Localization;
using Flashcards.Models;
using Flashcards.PlatformDependentTools;
using Flashcards.Services.DataAccess;
using Flashcards.Services.Http;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Forms;

namespace Flashcards.ViewModels
{
	public class AddFlashcardViewModel : INotifyPropertyChanged, INavigatedAware
	{
		private const int ImagesLimit = 30;
		private readonly IRepository<Flashcard> _flashcardRepository;
		private readonly IRepository<Lesson> _lessonRepository;
		private readonly IMessage _message;
		private readonly IPageDialogService _dialogService;
		private readonly IImageBrowser _imageBrowser;
		private readonly ITranslator _translator;
		private Lesson _lesson;

		public AddFlashcardViewModel()
		{
		}

		public AddFlashcardViewModel(
			ITranslator translator,
			IRepository<Flashcard> flashcardRepository,
			IImageBrowser imageBrowser,
			IRepository<Lesson> lessonRepository,
			IMessage message,
			IPageDialogService dialogService)
		{
			_translator = translator;
			_flashcardRepository = flashcardRepository;
			_imageBrowser = imageBrowser;
			_lessonRepository = lessonRepository;
			_message = message;
			_dialogService = dialogService;
		}

		public ObservableCollection<Uri> ImageUris { get; } = new ObservableCollection<Uri>(new Uri[ImagesLimit]);

		public Uri SelectedImageUri { get; set; }

		public string FrontText { get; set; }

		public string BackText { get; set; }

		public string FrontLanguage { get; private set; }
		public string BackLanguage { get; private set; }

		public ICommand NextFlashcardCommand => new Command(async () =>
		{
			if (FrontText == string.Empty || BackText == string.Empty)
			{
				_message.ShortAlert(AppResources.FlashcardCannotBeEmpty);
				return;
			}

			if (!(await _lessonRepository.FindWhere(l => l.Id == _lesson.Id)).Any())
			{
				await _lessonRepository.Insert(_lesson);
			}

			var flashcard = new Flashcard
			{
				Front = FrontText,
				Back = BackText,
				LessonId = _lesson.Id,
				ImageUrl = SelectedImageUri?.AbsoluteUri
			};

			var doNotAwait = _flashcardRepository.Insert(flashcard);

			FrontText = "";
			BackText = "";
			ImageUris.Clear();
			SelectedImageUri = null;
		});

		public ICommand ClearFront => new Command(() => FrontText = string.Empty);
		public ICommand ClearBack => new Command(() => BackText = string.Empty);

		public void OnNavigatedTo(NavigationParameters parameters)
		{
			_lesson = (Lesson) parameters["lesson"];
			FrontLanguage = _lesson.FrontLanguage.ToString();
			BackLanguage = _lesson.BackLanguage.ToString();
		}

		public void OnNavigatedFrom(NavigationParameters parameters)
		{
			parameters.Add("lesson", _lesson);
		}

#pragma warning disable 0067
		public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 0067

		private async Task UpdateImages(Language language, string text)
		{
			await Try(async () =>
			{
				var imageUris = await _imageBrowser.Find(text, language, ImagesLimit);
				ImageUris.Clear();
				foreach (var imageUri in imageUris) ImageUris.Add(imageUri);
			});
		}

		public async Task HandleFrontTextChangedByUser()
		{
			if(!string.IsNullOrEmpty(BackText))
				return;

			async Task UpdateTranslation()
			{
				await Try(async () =>
				{
					var translations = await _translator.Translate(
						from: _lesson.FrontLanguage,
						to: _lesson.BackLanguage,
						text: FrontText);
					BackText = string.Join("", translations);
				});
			}

			var updateImages = UpdateImages(_lesson.FrontLanguage, FrontText);

			await Task.WhenAll(UpdateTranslation(), updateImages);
		}

		public async Task HandleBackTextChangedByUser()
		{
			if(!string.IsNullOrEmpty(FrontText))
				return;

			async Task UpdateTranslation()
			{
				await Try(async () =>
				{
					var translations =
						await _translator.Translate(from: _lesson.BackLanguage, to: _lesson.FrontLanguage, text: BackText);
					FrontText = string.Join("", translations);
				});
			}
			var updateImages = UpdateImages(_lesson.BackLanguage, BackText);

			await Task.WhenAll(UpdateTranslation(), updateImages);
		}

		private async Task Try(Func<Task> action)
		{
			try
			{
				await action();
			}
			catch (HttpRequestException e)
			{
				Debug.WriteLine(e.ToString());
			}
			catch (Exception e)
			{
				var message = e.ToString();
				// workaround for not compiling unit tests when referencing Mono.Android
				if (message.StartsWith("Java.Net.UnknownHostException")) 
				{
					Debug.WriteLine(message);
					return;
				}

				await _dialogService.DisplayAlertAsync("Error", message, "OK");
			}
		}
	}
}