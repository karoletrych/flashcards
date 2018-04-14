using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Flashcards.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LessonListView : ContentView
	{
		public LessonListView ()
		{
			InitializeComponent ();
		}
	}
}