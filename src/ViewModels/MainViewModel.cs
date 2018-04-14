using System;
using System.ComponentModel;
using System.Windows.Input;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Forms;

namespace Flashcards.ViewModels
{
	public class MainViewModel : INavigatedAware
	{
		private readonly INavigationService _navigationService;

		public MainViewModel()
		{
			
		}

		public MainViewModel(
			INavigationService navigationService,
			IPageDialogService pageDialogService,
			Func<INavigationService, LessonListViewModel> lessonListViewModel,
			Func<INavigationService, IPageDialogService, RepetitionViewModel> repetitionViewModel)
		{
			LessonListViewModel = lessonListViewModel(navigationService);
			RepetitionViewModel = repetitionViewModel(navigationService, pageDialogService);
			_navigationService = navigationService;
		}

		public LessonListViewModel LessonListViewModel { get; }
		public RepetitionViewModel RepetitionViewModel { get; }

		public ICommand SettingsCommand => new Command(async () => { await _navigationService.NavigateAsync("SettingsPage"); });

		public void OnNavigatedFrom(NavigationParameters parameters)
		{
			LessonListViewModel.OnNavigatedFrom(parameters);
			RepetitionViewModel.OnNavigatedFrom(parameters);
		}

		public void OnNavigatedTo(NavigationParameters parameters)
		{
			LessonListViewModel.OnNavigatedTo(parameters);
			RepetitionViewModel.OnNavigatedTo(parameters);
		}
	}
}