using System.Collections.Generic;
using System.Collections.ObjectModel;
using Flashcards.ViewModels;
using Xamarin.Forms;

namespace Flashcards.Views.CustomViews
{
    [ContentProperty("StepItems")]
    public sealed class MulticolorBar : View
    {
        public static readonly BindableProperty StepItemsProperty =
            BindableProperty.Create(
                propertyName: "StepItems",
                returnType: typeof(ObservableCollection<ColorbarItem>),
                declaringType: typeof(MulticolorBar),
                defaultValue: new ObservableCollection<ColorbarItem>());

        public ObservableCollection<ColorbarItem> StepItems
        {
            get => (ObservableCollection<ColorbarItem>) GetValue(StepItemsProperty);
            set => SetValue(StepItemsProperty, value);
        }
    }
}