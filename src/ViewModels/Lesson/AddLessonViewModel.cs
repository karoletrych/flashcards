using System;
using System.Collections.Generic;
using FlashCards.Models.Dto;

namespace FlashCards.ViewModels.Lesson
{
    public class AddLessonViewModel
    {
        public IEnumerable<string> LanguageNames => Enum.GetNames(typeof(Language));
    }
}
