using System.Windows.Input;
using Xamarin.Forms;

namespace Flashcards.Views.CustomControls
{
    public class CommandListView : ListView
    {
        public static readonly BindableProperty ItemClickCommandProperty = BindableProperty.Create(
            nameof(ItemClickCommand),
            typeof(ICommand),
            typeof(CommandListView),
            default(ICommand));

        public CommandListView()
        {
            ItemTapped += OnItemTapped;
        }

        public ICommand ItemClickCommand
        {
            private get => (ICommand) GetValue(ItemClickCommandProperty);
            set => SetValue(ItemClickCommandProperty, value);
        }

        private void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item != null && ItemClickCommand != null && ItemClickCommand.CanExecute(e))
            {
                ItemClickCommand.Execute(e.Item);
                SelectedItem = null;
            }
        }
    }
}