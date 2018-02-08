using System;
using System.Collections.Generic;

namespace FlashCards.Model
{
    class Lesson
    {
        public IEnumerable<FlashCard> FlashCards { get; }
        public Language TopLanguage { get; }
        public Language BottomLanguage { get; }

        void AddFlashCard(string top, string bottom, Uri imageUri)
        {

        }
    }

    class FlashCard
    {
        public FlashCard(string top, string bottom, Uri imageUri, decimal strength, DateTime nextQuestionTime)
        {
            Top = top;
            Bottom = bottom;
            ImageUri = imageUri;
            Strength = strength;
            NextQuestionTime = nextQuestionTime;
        }

        public string Top { get; }
        public string Bottom { get; }
        public Uri ImageUri { get; }
        public decimal Strength { get; set; }
        public DateTime NextQuestionTime { get; }
    }
}
