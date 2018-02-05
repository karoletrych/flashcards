namespace FlashCards.Model
{
    internal class Question
    {
        public string QuestionText { get; }
        public string AnswerText { get; }
        public QuestionStatus Status { get; set; }
    }
}