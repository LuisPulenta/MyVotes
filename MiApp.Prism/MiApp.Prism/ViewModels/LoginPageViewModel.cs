using Newtonsoft.Json;
using Prism.Commands;
using Prism.Navigation;
using MiApp.Common.Helpers;
using MiApp.Common.Models;
using MiApp.Common.Services;
using MiApp.Prism.Views;
using System.Collections.Generic;

namespace MiApp.Prism.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;

        private bool _isRunning;
        public bool IsRunning { get => _isRunning; set => SetProperty(ref _isRunning, value); }

        private bool _isEnabled;
        public bool IsEnabled { get => _isEnabled; set => SetProperty(ref _isEnabled, value); }

        private bool _isRemember;
        public bool IsRemember { get => _isRemember; set => SetProperty(ref _isRemember, value); }

        private string _password;
        public string Password { get => _password; set => SetProperty(ref _password, value); }

        public string Email { get; set; }

        public List<VersionResponse> MyVersions { get; set; }

        private DelegateCommand _loginCommand;
        public DelegateCommand LoginCommand => _loginCommand ?? (_loginCommand = new DelegateCommand(LoginAsync));

        private DelegateCommand _registerCommand;
        public DelegateCommand RegisterCommand => _registerCommand ?? (_registerCommand = new DelegateCommand(RegisterAsync));

        private DelegateCommand _forgotPasswordCommand;
        public DelegateCommand ForgotPasswordCommand => _forgotPasswordCommand ?? (_forgotPasswordCommand = new DelegateCommand(ForgotPasswordAsync));


        public LoginPageViewModel(INavigationService navigationService, IApiService apiService)
            : base(navigationService)
        {
            _navigationService = navigationService;
            _apiService = apiService;
            IsRemember = true;
            IsEnabled = true;
            Title = "Login";
            IsEnabled = true;
            Email = "lucho@yopmail.com";
            Password = "123456";
        }

        private async void LoginAsync()
        {
            if (string.IsNullOrEmpty(Email))
            {
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "Debe ingresar un Usuario",
                    "Aceptar");
                return;
            }

            if (string.IsNullOrEmpty(Password))
            {
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "Debe ingresar un Password",
                    "Aceptar");
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

            TokenRequest request = new TokenRequest
            {
                Password = Password,
                Username = Email
            };

            Response response = await _apiService.GetTokenAsync(url, "Account", "/CreateToken", request);

            if (!response.IsSuccess)
            {
                IsRunning = false;
                IsEnabled = true;
                await App.Current.MainPage.DisplayAlert(
                   "Error",
                    "Usuario o Password incorrecto",
                    "Aceptar");
                Password = string.Empty;
                return;
            }

            TokenResponse token = (TokenResponse)response.Result;
            EmailRequest request2 = new EmailRequest
            {
                Email = Email
            };

            Response response2 = await _apiService.GetUserByEmail(
                url,
                "api",
                "/Account/GetUserByEmail",
                "bearer",
                token.Token, request2);

            UserResponse userResponse = (UserResponse)response2.Result;

            Settings.User = JsonConvert.SerializeObject(userResponse);
            Settings.Token = JsonConvert.SerializeObject(token);
            Settings.IsLogin = true;

            //********** CONTROLA NUMERO DE VERSION **********
            var response3 = await _apiService.GetListTAsync<VersionResponse>(
                 url,
                 "api",
                 "/Versions");

            this.MyVersions = (List<VersionResponse>)response3.Result;



            if (response3.IsSuccess)
            {
                var bandera = 0;
                foreach (var cc in MyVersions)
                {
                    if (cc.NroVersion != "1.1")
                    {
                        bandera = 1;
                    }
                }

                if (bandera == 1)
                {
                    //Avisar que hay una nueva version
                    await App.Current.MainPage.DisplayAlert(
                   "Aviso",
                    "Hay una nueva versión en Google Play para descargar",
                    "Aceptar");
                }
            }

            IsRunning = false;
            IsEnabled = true;

            await _navigationService.NavigateAsync("/MiAppMasterDetailPage/NavigationPage/ItemsPage");
            Password = string.Empty;
        }
        private async void RegisterAsync()
        {
            await _navigationService.NavigateAsync(nameof(RegisterPage));
        }

        private async void ForgotPasswordAsync()
        {
            await _navigationService.NavigateAsync(nameof(RememberPasswordPage));
        }


    }
}