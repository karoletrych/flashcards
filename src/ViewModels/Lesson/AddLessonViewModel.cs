using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Timers;
using FlashCards.Models;
using FlashCards.Models.Dto;
using FlashCards.ViewModels.Annotations;

namespace FlashCards.ViewModels.Lesson
{
    public class AddLessonViewModel : INotifyPropertyChanged
    {
        private const int TranslationDelayInMilliseconds = 800;
        private readonly ITranslator _translator;
        private string _backText;
        private string _frontText;

        public AddLessonViewModel(ITranslator translator)
        {
            _translator = translator;
            _timer.Elapsed += TimerOnElapsed;
            _timerBack.Elapsed += TimerOnElapsedBack;
        }

        public IList<string> LanguageNames =>
            Enum.GetNames(typeof(Language))
                .OrderBy(language => language).ToList();

        public string SelectedFrontLanguage { get; set; }
        public string SelectedBackLanguage { get; set; }

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

        private readonly Timer _timer = new Timer(TranslationDelayInMilliseconds)
        {
            AutoReset = false
        };

        private readonly Timer _timerBack = new Timer(TranslationDelayInMilliseconds)
        {
            AutoReset = false
        };

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
                from: SelectedFrontLanguage.ToLanguageEnum(),
                to: SelectedBackLanguage.ToLanguageEnum(),
                text: FrontText);
            BackText = string.Join("", translations);
        }

        private async void TimerOnElapsedBack(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            var translations = await _translator.Translate(
                from: SelectedBackLanguage.ToLanguageEnum(),
                to: SelectedFrontLanguage.ToLanguageEnum(),
                text: BackText);
            FrontText = string.Join("", translations);
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}