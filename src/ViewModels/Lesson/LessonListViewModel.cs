using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Flashcards.Models;
using Flashcards.Services.Database;
using Nito.Mvvm;
using Prism.Navigation;
using Xamarin.Forms;

namespace Flashcards.ViewModels.Lesson
{
    public class LessonListViewModel : INotifyPropertyChanged
    {
        private readonly IRepository<Models.Lesson> _lessonRepository;
        private readonly INavigationService _navigationService;

        public LessonListViewModel(
            IRepository<Models.Lesson> lessonRepository,
            INavigationService navigationService)
        {
            _lessonRepository = lessonRepository;
            _navigationService = navigationService;
        }

        public NotifyTask<IEnumerable<Models.Lesson>> Items => NotifyTask.Create(async () => await _lessonRepository.FindAll());

        public ICommand AddLesson => new Command(() => { _navigationService.NavigateAsync("AddLessonPage"); });

        public event PropertyChangedEventHandler PropertyChanged;
    }
}