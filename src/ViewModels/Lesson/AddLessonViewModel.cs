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
    public class AddLessonViewModel
    {

        public IList<string> LanguageNames =>
            Enum.GetNames(typeof(Language))
                .OrderBy(language => language).ToList();

        public string SelectedFrontLanguage { get; set; }
        public string SelectedBackLanguage { get; set; }
    }
}