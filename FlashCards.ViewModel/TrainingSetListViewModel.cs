using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace FlashCards.ViewModel
{
    internal class TrainingSetListViewModel
    {
        public IEnumerable<TrainingSetViewModel> Items { get; set; } =
            new List<TrainingSetViewModel>
            {
                
            };
    }

    internal class TrainingSetViewModel
    {
        public string Name { get; set; }
        public int QuestionsCount { get; set; }
        public string Languages { get; set; }

        private readonly INavigationService _navigationService;

        public TrainingSetViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public ICommand OpenQuestionsPage
        {
            get
            {
                return new Command(() => _navigationService.NavigateTo(new object()));
            }
        }
    }
}