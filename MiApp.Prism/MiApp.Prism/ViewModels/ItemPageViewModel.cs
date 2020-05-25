using Prism.Navigation;
using MiApp.Common.Models;
using Newtonsoft.Json;
using MiApp.Common.Helpers;

namespace MiApp.Prism.ViewModels
{
    public class ItemPageViewModel : ViewModelBase
    {
        private ItemResponse _item;

        public ItemPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = "Item";
        }

        public ItemResponse Item
        {
            get => _item;
            set => SetProperty(ref _item, value);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.ContainsKey("item"))
            {
                // Recuperar parametro pasado con la Clase Settings
                //_item = JsonConvert.DeserializeObject<ItemResponse>(Settings.Item);

                Item = parameters.GetValue<ItemResponse>("item");
                Title = Item.Name;
            }
        }
    }
}