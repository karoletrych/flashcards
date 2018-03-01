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
        private const int TranslationDelayInMilliseconds = 800;

        private readonly Timer _timer = new Timer(TranslationDelayInMilliseconds)
        {
            AutoReset = false
        };

        private readonly Timer _timerBack = new Timer(TranslationDelayInMilliseconds)
        {
            AutoReset = false
        };

        private readonly ITranslatorService _translatorService;
        private readonly IRepository<Flashcard> _flashcardRepository;
        private Language _backLanguage;

        private string _backText;
        private Language _frontLanguage;

        private string _frontText;
        private int _lessonId;

        public AddFlashcardViewModel(ITranslatorService translatorService, IRepository<Flashcard> flashcardRepository)
        {
            _translatorService = translatorService;
            _flashcardRepository = flashcardRepository;
            _timer.Elapsed += TimerOnElapsed;
            _timerBack.Elapsed += TimerOnElapsedBack;
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

        public ICommand NextFlashcard => new Command(async () =>
        {
            await _flashcardRepository.Insert(new Flashcard{Front = FrontText, Back = BackText, LessonId = _lessonId});
            FrontText = "";
            BackText = "";
        });

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            _frontLanguage = (Language) parameters["frontLanguage"];
            _backLanguage = (Language) parameters["backLanguage"];
            _lessonId = (int) parameters["lessonId"];
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void ResetTimer()
        {
            _timer.Stop();
            _timer.Start();
        }

        private void ResetTimerBack()
        {
            _timerBack.Stop();
            _timerBack.Start();
        }

        public void FrontTextModified()
        {
            ResetTimer();
        }

        public void BackTextModified()
        {
            ResetTimerBack();
        }

        private async void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            var translations = await _translatorService.Translate(
                from: _frontLanguage,
                to: _backLanguage,
                text: FrontText);
            BackText = string.Join("", translations);
        }

        private async void TimerOnElapsedBack(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            var translations = await _translatorService.Translate(
                from: _backLanguage,
                to: _frontLanguage,
                text: BackText);
            FrontText = string.Join("", translations);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}