using System.Collections.ObjectModel;
using System.Windows.Input;
using Windows.Devices.Geolocation;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using ShoppingListWPApp.Common;
using ShoppingListWPApp.Models;

namespace ShoppingListWPApp.ViewModels
{
    class MainPageViewModel : ViewModelBase
    {
        #region *** Private Members ***

        private INavigationService navigationService;
        private IDialogService dialogService;
        private Geolocator locator;

        #endregion

        #region *** Properties ***

        public ObservableCollection<Shop> Shops { get; set; }

        public Shop SelectedShop { get; set; }

        public ICommand AddShopCommand { get; set; }
        public ICommand DetailsCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        #endregion

        public MainPageViewModel(INavigationService navigationService, IDialogService dialogService, Geolocator locator)
        {
            // Services
            this.navigationService = navigationService;
            this.dialogService = new DialogService();
            this.locator = locator;

            Shops = new ObservableCollection<Shop>();
            AddShopCommand = new RelayCommand(GoToAddShopPage);
        }

        public void AddShop(Shop shop)
        {
            Shops.Add(shop);
        }

        private void GoToAddShopPage()
        {
            navigationService.NavigateTo("addShop");
        }
    }
}
