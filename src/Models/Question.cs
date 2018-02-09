namespace Models
{
    public class Question
    {
        public Question(string questionText, string answerText)
        {
            QuestionText = questionText;
            AnswerText = answerText;
        }

        public string QuestionText { get; }
        public string AnswerText { get; }
        public QuestionStatus Status { get; set; }
    }
}