using System.Collections.Generic;
using Xamarin.Forms;

namespace FlashCards
{
    [ContentProperty("StepItems")]
    public sealed class MulticolorBar : View
	{
		public static readonly BindableProperty StepItemsProperty =
			BindableProperty.Create(
				propertyName: "StepItems",
				returnType: typeof(IList<StepItem>),
				declaringType: typeof(MulticolorBar),
				defaultValue: new List<StepItem>());
        
        

		public IList<StepItem> StepItems
		{
			get => (IList<StepItem>) GetValue(property: StepItemsProperty);
			set => SetValue(property: StepItemsProperty, value: value);
		}
	}
}