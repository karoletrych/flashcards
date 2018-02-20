using System.Collections.Generic;
using System.Windows.Input;
using FlashCards.Services.Database;
using Xamarin.Forms;

namespace FlashCards.ViewModels.Lesson
{
    public class LessonListViewModel
    {
        private readonly IRepository<Models.Lesson> _modelsRepository;
        private readonly INavigationService _navigationService;

        public LessonListViewModel(IRepository<Models.Lesson> modelsRepository, INavigationService navigationService)
        {
            _modelsRepository = modelsRepository;
            _navigationService = navigationService;
        }

        public IEnumerable<LessonViewModel> Items;// =
//            _modelsRepository.FindAll().Result.Select(l => new LessonViewModel(l));

        private ICommand AddLesson => new Command(() => _navigationService.NavigateTo("AddLessonPage"));
    }
}