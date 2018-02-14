using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Timers;
using FlashCards.Models;
using FlashCards.Services;
using FlashCards.ViewModels.Annotations;

namespace FlashCards.ViewModels
{
    public class AddFlashCardViewModel : INotifyPropertyChanged
    {
        public delegate AddFlashCardViewModel
            Factory(Language frontLanguage, Language backLanguage);

        private const int TranslationDelayInMilliseconds = 800;
        private readonly Language _frontLanguage;
        private readonly Language _backLanguage;

        private readonly Timer _timer = new Timer(TranslationDelayInMilliseconds)
        {
            AutoReset = false
        };

        private readonly Timer _timerBack = new Timer(TranslationDelayInMilliseconds)
        {
            AutoReset = false
        };

        private readonly ITranslator _translator;


        private string _backText;

        private string _frontText;

        public AddFlashCardViewModel(ITranslator translator, Language backLanguage, Language frontLanguage)
        {
            _translator = translator;
            _backLanguage = backLanguage;
            _frontLanguage = frontLanguage;
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
            var translations = await _translator.Translate(
                from: _frontLanguage,
                to: _backLanguage,
                text: FrontText);
            BackText = string.Join("", translations);
        }

        private async void TimerOnElapsedBack(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            var translations = await _translator.Translate(
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