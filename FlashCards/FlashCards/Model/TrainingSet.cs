using System.Collections.Generic;
using FlashCards.ViewModel;

namespace FlashCards.Model
{
    internal class TrainingSet
    {
        public IEnumerable<QuestionStatus> QuestionsStatus =>
            new[]
            {
                QuestionStatus.AnsweredCorrectly,
                QuestionStatus.AnsweredWrongly,
                QuestionStatus.AnsweredCorrectly,
                QuestionStatus.NotAsked,
                QuestionStatus.NotAsked,
                QuestionStatus.NotAsked,
                QuestionStatus.NotAsked
            };

        public void AnswerKnow()
        {
        }

        public void AnswerDontKnow()
        {
        }

        public Question GetNextQuestion()
        {
            return new Question();
        }
    }

    internal class Question
    {
        public string QuestionText { get; set; }
        public string AnswerText { get; set; }
    }
}