using Prism.Commands;
using Prism.Navigation;
using MiApp.Common.Models;
using MiApp.Common.Helpers;
using Newtonsoft.Json;
using MiApp.Common.Services;

namespace MiApp.Prism.ViewModels
{
    public class ItemItemViewModel : ItemResponse
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private DelegateCommand _selectItemCommand;
        public DelegateCommand SelectItemCommand => _selectItemCommand ?? (_selectItemCommand = new DelegateCommand(SelectItemAsync));

        private DelegateCommand _deleteItemCommand;
        public DelegateCommand DeleteItemCommand => _deleteItemCommand ?? (_deleteItemCommand = new DelegateCommand(DeleteItem));

        private DelegateCommand _editItemCommand;
        public DelegateCommand EditItemCommand => _editItemCommand ?? (_editItemCommand = new DelegateCommand(EditItem));

        public ItemItemViewModel(INavigationService navigationService,IApiService apiService)
        {
            _navigationService = navigationService;
            _apiService = apiService;
        }



        private async void SelectItemAsync()
        {
            NavigationParameters parameters = new NavigationParameters
            {
                { "item", this }
            };

            Settings.Item = JsonConvert.SerializeObject(this);
            await _navigationService.NavigateAsync("ItemPage", parameters);
        }


        private async void DeleteItem()
        {
            var answer = await App.Current.MainPage.DisplayAlert(
               "Confirmar",
               "¿Está seguro de borrar este Item?",
               "Si",
               "No");

            if (!answer)
            {
                return;
            }

            var url = App.Current.Resources["UrlAPI"].ToString();
            var token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);

            var response = await _apiService.DeleteAsync(
               url,
               "api",
               "/Items",
               this.Id,
               "bearer",
               token.Token);

            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "No se puedo borrar", //response.Message,
                    "Accept");
                return;
            }
            ItemsPageViewModel.GetInstance().LoadItemsAsync();
        }

        private async void EditItem()
        {
            Settings.Item = JsonConvert.SerializeObject(this);
            await _navigationService.NavigateAsync("EditItemPage");
        }
    }
}