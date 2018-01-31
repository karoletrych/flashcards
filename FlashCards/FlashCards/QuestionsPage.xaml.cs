using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FlashCards
{
    public partial class QuestionsPage : ContentPage
	{
		public QuestionsPage()
		{
			InitializeComponent();
		}

	    void OnShowAnswerButtonClicked(object sender, EventArgs args)
	    {
	        QuestionAnswer.Text = "ANSWER";

	        ShowAnswer.IsVisible = false;
	        DontKnow.IsVisible = true;
	        Know.IsVisible = true;
	    }

	    private int goodAnswers = 0;
	    private int badAnswers = 0;

	    private void LoadNextQuestion()
	    {
	        //            if(goodAnswers == 0)
	        //                ProgressBar.Progress = 1;
	        //	        else if (badAnswers == 0)
	        //	            ProgressBar.Progress = 0;
	        //            else
	        //            {
	        //                var ratio = (float)goodAnswers / badAnswers;
	        //                ProgressBar.Progress = ratio;
	        //            }
	        //
	        //            ProgressBar.Progress += 0.1;
	        QuestionImage.Source = "icon.png";

	        ShowAnswer.IsVisible = true;
	        Know.IsVisible = false;
	        DontKnow.IsVisible = false;
	    }

	    void OnKnowButtonClicked(object sender, EventArgs args)
	    {
	        goodAnswers++;
	        LoadNextQuestion();
	    }

	    void OnDontKnowButtonClicked(object sender, EventArgs args)
	    {
	        badAnswers++;
	        LoadNextQuestion();
	    }
    }
}
