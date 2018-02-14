using System;
using System.Collections.Generic;
using System.Linq;
using FlashCards.Models;

namespace FlashCards.ViewModels.Lesson
{
    public class AddLessonViewModel
    {

        public IList<string> LanguageNames =>
            Enum.GetNames(typeof(Language))
                .OrderBy(language => language).ToList();

        public string SelectedFrontLanguage { get; set; }
        public string SelectedBackLanguage { get; set; }
    }
}