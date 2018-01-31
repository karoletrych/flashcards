using Android.Content;
using Android.Graphics;
using FlashCards;
using FlashCards.Droid.MulticolorBar;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(MulticolorBar), typeof(MulticolorBarRenderer))]
namespace FlashCards.Droid.MulticolorBar
{
	public class MulticolorBarRenderer : ViewRenderer<FlashCards.MulticolorBar, AndroidMulticolorBar>
	{
		private AndroidMulticolorBar _multicolorBar;

		public MulticolorBarRenderer(Context context) : base(context)
		{
		}

		protected override void OnElementChanged(ElementChangedEventArgs<FlashCards.MulticolorBar> e)
		{
			base.OnElementChanged(e);

			SetWillNotDraw(false);
			if (Control == null)
			{
				_multicolorBar = new AndroidMulticolorBar(Context);
				SetNativeControl(_multicolorBar);
			}

		    if (e.OldElement != null)
		    {
		        // Unsubscribe from event handlers and cleanup any resources
		    }

            if (e.NewElement != null)
            {
                _multicolorBar.StepItems = e.NewElement.StepItems;
                // Configure the control and subscribe to event handlers
            }
        }

		protected override void OnDraw(Canvas canvas)
		{
			base.OnDraw(canvas);
			_multicolorBar.Draw(canvas);

		}
	}
}