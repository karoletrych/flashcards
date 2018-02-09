using System;
using System.Collections.Generic;

namespace Models
{
    enum Language
    {
        German,
        English,
        Polish,
        French,
        Italian,
        Spanish,
        Swedish,
        Norwegian
    }

    class Lesson
    {
        public IEnumerable<FlashCard> FlashCards { get; }
        public Language TopLanguage { get; }
        public Language BottomLanguage { get; }

        void AddFlashCard(string top, string bottom, Uri imageUri)
        {

        }
    }

    internal class FlashCard
    {
        public int FlashCardId { get; set; }
        public string Top { get; set; }
        public string Bottom { get; set; }
    }
}
