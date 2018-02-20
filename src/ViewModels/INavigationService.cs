using System;
using System.Collections.Generic;
using System.Text;

namespace FlashCards.ViewModels
{
    public interface INavigationService
    {
        void NavigateTo(string pageName);
    }
}
