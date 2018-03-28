using System;
using System.Collections.Generic;

namespace Flashcards.Services.Examiner
{
    public class QuestionResultsEventArgs : EventArgs
    {
        public QuestionResultsEventArgs(IList<AnsweredQuestion> results)
        {
            Results = results;
        }
        
        public IList<AnsweredQuestion> Results { get; }
    }
}