using System.Collections.Generic;
using FlashCards.ViewModel;

namespace FlashCards.Model
{
    class TrainingSet
    {
        public IEnumerable<QuestionStatus> QuestionsStatus => new[] { QuestionStatus.AnsweredCorrectly, QuestionStatus.AnsweredWrong, QuestionStatus.AnsweredCorrectly, QuestionStatus.NotAsked, QuestionStatus.NotAsked, QuestionStatus.NotAsked, QuestionStatus.NotAsked };

        public void AnswerKnow()
        {

        }

        public void AnswerDontKnow()
        {

        }

        public Question GetQuestion()
        {
            return new Question();
        }
    }

    internal class Question
    {
    }
}
