using MiApp.Prism.ViewModels;
using Xamarin.Forms;

namespace MiApp.Prism.Views
{
    public partial class ItemsPage : ContentPage
    {
        public ItemsPage()
        {
            InitializeComponent();
        }
        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            var itemsPage = ItemsPageViewModel.GetInstance();
            itemsPage.RefreshList();
        }
    }
}
