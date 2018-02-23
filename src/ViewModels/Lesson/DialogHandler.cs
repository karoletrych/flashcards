using System;
using Prism.Services;

namespace Flashcards.ViewModels.Lesson
{
    static class DialogHandler
    {
        public static async void HandleExceptions(IPageDialogService dialogService, Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                await dialogService.DisplayAlertAsync("Something is no yes", e.ToString(), "OK");
            }
        }
    }
}