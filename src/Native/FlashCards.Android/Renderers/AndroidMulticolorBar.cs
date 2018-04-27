using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Flashcards.ViewModels;
using Flashcards.ViewModels.Tools;

namespace FlashCards.Droid.Renderers
{
	public class AndroidMulticolorBar : View
	{
		public AndroidMulticolorBar(Context context) : base(context)
		{
		}

		private const int ProgressBarHeight = 50;
		public IList<MulticolorbarItem> Items { private get; set; }

		protected override void OnDraw(Canvas canvas)
		{
			base.OnDraw(canvas);

			var paint = new Paint();

			var barEnd = 0;
			var totalLength = Items.Select(item => item.Value).Sum();

			foreach (var stepItem in Items)
			{
				var (a, r, g, b) = ConvertColorToInteger(stepItem.Color);
				paint.SetARGB(a, r, g, b);
				var fraction = (double)stepItem.Value / totalLength;
				var stepItemWidth = (int)Math.Round(fraction * Width);
				var stepItemRectangle = new Rect(left: barEnd, right: barEnd + stepItemWidth, top: 0,
					bottom: ProgressBarHeight);
				canvas.DrawRect(stepItemRectangle, paint);
				barEnd += stepItemWidth;
			}
		}

		private (int a, int r, int g, int b) ConvertColorToInteger(Color color)
		{
			return ((int)(color.A * 255), (int)(color.R * 255), (int)(color.G * 255), (int)(color.B * 255));
		}
	}
}