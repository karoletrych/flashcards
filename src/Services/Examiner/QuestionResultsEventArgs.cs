using System;
using System.Collections.Generic;

namespace Flashcards.Services.Examiner
{
    public class QuestionResultsEventArgs : EventArgs
    {
        public QuestionResultsEventArgs(IList<AnsweredQuestion> results, int numberOfQuestionsInNextSession)
        {
	        Results = results;
	        NumberOfQuestionsInNextSession = numberOfQuestionsInNextSession;
        }
        
        public IList<AnsweredQuestion> Results { get; }
	    public int NumberOfQuestionsInNextSession { get; }
    }
}