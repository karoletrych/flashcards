using Flashcards.Models;

namespace Flashcards.Services
{
    public class Question
    {
        public Question(Flashcard flashcard)
        {
            Flashcard = flashcard;
        }

        public Flashcard Flashcard { get; }
        public QuestionStatus Status { get; set; }
    }
}