using System.Collections.Generic;
using FlashCards.ViewModels;
using Xamarin.Forms;

namespace FlashCards.Views.MulticolorBar
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
            get => (IList<StepItem>) GetValue(StepItemsProperty);
            set => SetValue(StepItemsProperty, value);
        }
    }
}