using System.Windows.Input;
using Xamarin.Forms;

namespace FlashCards.ViewModel
{
    public class CommandViewCell : ViewCell
    {
        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create("Command",
                typeof(ICommand),
                typeof(CommandViewCell),
                null,
                propertyChanged: OnCommandPropertyChanged);

        public static readonly BindableProperty CommandParameterProperty =
            BindableProperty.Create("CommandParameter",
                typeof(object),
                typeof(CommandViewCell),
                null,
                propertyChanged: OnCommandParameterPropertyChanged);

        public ICommand Command
        {
            get => (ICommand) GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public object CommandParameter
        {
            get => (ICommand) GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        private static void OnCommandParameterPropertyChanged(BindableObject bindable, object oldValue,
            object newValue)
        {
            //Stuff to handle changes, not really required
        }

        private static void OnCommandPropertyChanged(BindableObject bindable, object oldValue,
            object newValue)
        {
            //More stuff to handle changes
        }

        protected override void OnTapped()
        {
            if (Command != null)
            {
                var parameter = CommandParameter;
                Command.Execute(parameter);
            }
        }
    }
}