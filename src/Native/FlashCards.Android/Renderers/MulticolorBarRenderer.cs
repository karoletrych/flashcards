﻿using System;
using System.ComponentModel;
using Android.Content;
using Android.Graphics;
using Flashcards.Views.CustomViews;
using Flashcards.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(MulticolorBar), typeof(MulticolorBarRenderer))]
namespace Flashcards.Droid.Renderers
{
	public class MulticolorBarRenderer : ViewRenderer<MulticolorBar, AndroidMulticolorBar>
	{
		private AndroidMulticolorBar _multicolorBar;

		public MulticolorBarRenderer(Context context) : base(context)
		{
		}

		protected override void OnElementChanged(ElementChangedEventArgs<MulticolorBar> e)
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
				e.OldElement.ColorbarItemsChanged -= OnInvalidate;
			}

			if (e.NewElement != null)
			{
				_multicolorBar.Items = e.NewElement.ItemsSource;
				e.NewElement.ColorbarItemsChanged += OnInvalidate;
			}
		}

		private void OnInvalidate(object sender, EventArgs e)
		{
			Invalidate();
		}

		protected override void OnDraw(Canvas canvas)
		{
			base.OnDraw(canvas);
			_multicolorBar.Draw(canvas);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
			Invalidate();
		}
	}
}