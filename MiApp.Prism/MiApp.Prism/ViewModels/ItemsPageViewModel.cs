using Prism.Navigation;
using MiApp.Common.Models;
using MiApp.Common.Services;
using System.Collections.Generic;
using System.Linq;
using Prism.Commands;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using MiApp.Common.Helpers;

namespace MiApp.Prism.ViewModels
{
    public class ItemsPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;

        private bool _isRunning;
        public bool IsRunning { get => _isRunning; set => SetProperty(ref _isRunning, value); }

        private bool _isEnabled;
        public bool IsEnabled { get => _isEnabled; set => SetProperty(ref _isEnabled, value); }

        private bool _isRefreshing;
        public bool IsRefreshing { get => _isRefreshing; set => SetProperty(ref _isRefreshing, value); }

        private string _filter; 
        public string Filter { get => _filter; set => SetProperty(ref _filter, value);}

        private int _cantItems;
        public int CantItems { get => _cantItems; set => SetProperty(ref _cantItems, value);}

        private static ItemsPageViewModel _instance;

        private ItemResponse _item;
        public ItemResponse Item {get => _item;set => SetProperty(ref _item, value);}

        private ObservableCollection<ItemItemViewModel> _items;
        public ObservableCollection<ItemItemViewModel> Items { get => _items; set => SetProperty(ref _items, value);}

        public List<ItemResponse> MyItems { get; set; }

        private DelegateCommand _searchCommand;
        public DelegateCommand SearchCommand => _searchCommand ?? (_searchCommand = new DelegateCommand(Search));

        private DelegateCommand _refreshCommand;
        public DelegateCommand RefreshCommand => _refreshCommand ?? (_refreshCommand = new DelegateCommand(Refresh));

        private DelegateCommand _addItemCommand;
        public DelegateCommand AddItemCommand => _addItemCommand ?? (_addItemCommand = new DelegateCommand(AddItem));

        
        public static ItemsPageViewModel GetInstance()
        {
            return _instance;
        }


        public ItemsPageViewModel(
            INavigationService navigationService,
            IApiService apiService) : base(navigationService)
        {
            _navigationService = navigationService;
            _apiService = apiService;
            _instance = this;
            Title = "Items";
            LoadItemsAsync();
        }

        
        public async void LoadItemsAsync()
        {
            IsRefreshing = true;
            var token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);
            var url = App.Current.Resources["UrlAPI"].ToString();
            var controller = $"/Items";

            Response response = await _apiService.GetListAsync<ItemResponse>(
                url,
                "api",
                controller,
                 "bearer",
                 token.Token);

            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    response.Message,
                    "Aceptar");
                return;
            }

            MyItems = (List<ItemResponse>)response.Result;
            IsRefreshing = false;
            RefreshList();
            IsRefreshing = false;
        }

        public void RefreshList()
        {
            IsRefreshing = true;
            if (string.IsNullOrEmpty(this.Filter))
            {

                var myListItemsViewModel = MyItems.Select(t => new ItemItemViewModel(_navigationService, _apiService)
                {
                    Active = t.Active,
                    Date = t.Date,
                    Id = t.Id,
                    LogoPath = t.LogoPath,
                    Name = t.Name,
                    Price = t.Price,
                    Quantity = t.Quantity,
                });

                Items = new ObservableCollection<ItemItemViewModel>(myListItemsViewModel
                 .OrderBy(o => o.Name));

                CantItems = Items.Count();
            }
            else
            {
                var myListItemsViewModel = MyItems.Select(t => new ItemItemViewModel(_navigationService,_apiService)
                {
                    Active = t.Active,
                    Date = t.Date,
                    Id = t.Id,
                    LogoPath = t.LogoPath,
                    Name = t.Name,
                    Price = t.Price,
                    Quantity = t.Quantity,
                });

                Items = new ObservableCollection<ItemItemViewModel>(myListItemsViewModel
                 .OrderBy(o => o.Name)
                .Where(
                            t => (t.Name.ToLower().Contains(Filter.ToLower()))
                            ||
                            (t.Name.ToLower().Contains(Filter.ToLower()))
                            ));

                CantItems = Items.Count();
            }
            IsRefreshing = false;
        }

        private void Search()
        {
            RefreshList();
        }

        private void Refresh()
        {
            LoadItemsAsync();
        }

        private async void AddItem()
        {
            await _navigationService.NavigateAsync("AddItemPage");
        }
    }
}