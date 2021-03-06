﻿using MiApp.Common.Helpers;
using MiApp.Common.Models;
using MiApp.Common.Services;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Prism.Commands;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MiApp.Prism.ViewModels
{
    public class AddItemPageViewModel : ViewModelBase
    {
        private bool _isRunning;
        public bool IsRunning { get => _isRunning; set => SetProperty(ref _isRunning, value); }

        private bool _isEnabled;
        public bool IsEnabled { get => _isEnabled; set => SetProperty(ref _isEnabled, value); }

        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        

        private ImageSource _image;
        public ImageSource Image { get => _image;set => SetProperty(ref _image, value);}


        private DelegateCommand _changeImageCommand;
        public DelegateCommand ChangeImageCommand => _changeImageCommand ?? (_changeImageCommand = new DelegateCommand(ChangeImageAsync));


        private DelegateCommand _saveCommand;
        public DelegateCommand SaveCommand => _saveCommand ?? (_saveCommand = new DelegateCommand(Save));

        private MediaFile _file;


        private string _name;
        public string Name { get => _name; set => SetProperty(ref _name, value);}

        private DateTime _itemDate;
        public DateTime ItemDate { get => _itemDate; set => SetProperty(ref _itemDate, value); }

        private int _quantity;
        public int Quantity { get => _quantity; set => SetProperty(ref _quantity, value); }

        private double _price;
        public double Price { get => _price; set => SetProperty(ref _price, value); }

        public DateTime Hoy { get; set; }


        private static AddItemPageViewModel instance;
        public static AddItemPageViewModel GetInstance()
        {
            return instance;
        }


        public AddItemPageViewModel(INavigationService navigationService, IApiService apiService) : base(navigationService)
        {
            _navigationService = navigationService;
            _apiService = apiService;
            
            Hoy = DateTime.Today;
            ItemDate = DateTime.Today;
           
            instance = this;
            Image = App.Current.Resources["UrlNoImage"].ToString();

            IsEnabled = true;
            Title = "Agregar Nuevo Item";

        }






        private async void ChangeImageAsync()
        {
            await CrossMedia.Current.Initialize();

            string source = await Application.Current.MainPage.DisplayActionSheet(
                 "¿De dónde quiere tomar la foto?:",
                "Cancelar",
                null,
                "Galería",
                "Cámara");

            if (source == "Cancelar")
            {
                _file = null;
                return;
            }

            if (source == "Cámara")
            {
                _file = await CrossMedia.Current.TakePhotoAsync(
                    new StoreCameraMediaOptions
                    {
                        Directory = "Sample",
                        Name = "test.jpg",
                        PhotoSize = PhotoSize.Small,
                    }
                );
            }
            else
            {
                _file = await CrossMedia.Current.PickPhotoAsync();
            }

            if (_file != null)
            {
                Image = ImageSource.FromStream(() =>
                {
                    System.IO.Stream stream = _file.GetStream();
                    return stream;
                });
            }
        }

        private async void Save()
        {
            if (string.IsNullOrEmpty(this.Name))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Debe ingresar el nombre del Item.", "Aceptar");
                return;
            }

            IsRunning = true;
            IsEnabled = false;

            string url = App.Current.Resources["UrlAPI"].ToString();
            if (!_apiService.CheckConnection())
            {
                IsRunning = true;
                IsEnabled = false;
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "Revise su conexión a Internet",
                    "Aceptar");
                return;
            }


            byte[] ImageArray = null;
            if (_file != null)
            {
                ImageArray = FilesHelper.ReadFully(this._file.GetStream());
            }
            

            
            var token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);

            var myitem = new ItemRequest
            {
                Date = ItemDate,
                PictureArray = ImageArray,
                Name = Name,
                Active=true,
                Price=Price,
                Quantity=Quantity,                
            };

            var response = await _apiService.PostAsync(
            url,
            "api",
            "/Items",
            myitem,
            "bearer",
            token.Token);



            if (!response.IsSuccess)
            {
                IsRunning = false;
                IsEnabled = true;

                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    response.Message,
                    "Aceptar");
                return;
            }

            var myItem = (ItemRequest)response.Result;

            IsRunning = false;
            IsEnabled = true;


            await App.Current.MainPage.DisplayAlert(
                "Ok",
                "Guardado con éxito!!",
                "Aceptar");

            ItemsPageViewModel itemsPageViewModel = ItemsPageViewModel.GetInstance();
            itemsPageViewModel.LoadItemsAsync();
            itemsPageViewModel.RefreshList();

            await _navigationService.GoBackAsync();
        }
    }
}