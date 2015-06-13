using System.Collections.ObjectModel;
using System.Windows.Input;
using Windows.ApplicationModel.Resources;
using Windows.Devices.Geolocation;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ServiceLocation;
using ShoppingListWPApp.Models;

namespace ShoppingListWPApp.ViewModels
{
    class MainPageViewModel : ViewModelBase
    {
        #region *** Private Members ***

        private INavigationService navigationService;
        private IDialogService dialogService;
        private Geolocator locator;
        private Shop selectedShop;

        #endregion

        #region *** Properties ***

        public ObservableCollection<Shop> Shops { get; set; }

        public ObservableCollection<ShoppingList> ShoppingLists { get; set; }

        public Shop SelectedShop
        {
            get { return selectedShop; }
            set { selectedShop = value; }
        }

        public ICommand AddShopCommand { get; set; }
        public ICommand DetailsShopCommand { get; set; }
        public ICommand EditShopCommand { get; set; }
        public ICommand DeleteShopCommand { get; set; }
        public ICommand AddShoppingListCommand { get; set; }
        public ICommand AddShoppingListItemCommand { get; set; }

        #endregion

        public MainPageViewModel(INavigationService navigationService, IDialogService dialogService, Geolocator locator)
        {
            // Services
            this.navigationService = navigationService;
            this.dialogService = new DialogService();
            this.locator = locator;

            Shops = new ObservableCollection<Shop>();
            ShoppingLists = new ObservableCollection<ShoppingList>();

            AddShopCommand = new RelayCommand(GoToAddShopPage);
            EditShopCommand = new RelayCommand(GoToEditShopPage);
            DeleteShopCommand = new RelayCommand(GoToDeleteShop);
            DetailsShopCommand = new RelayCommand(GoToDetailsShop);
            AddShoppingListCommand = new RelayCommand(GoToAddShoppingListPage);
            AddShoppingListItemCommand = new RelayCommand<ShoppingList>(GoToShoppingListItem);
        }

        public void AddShop(Shop shop)
        {
            Shops.Add(shop);
        }

        public void EditShop(Shop oldShop, Shop newShop)
        {
            int idx = Shops.IndexOf(oldShop);

            Shops.Remove(oldShop);
            Shops.Insert(idx, newShop);
        }

        public void DeleteShop(Shop shop)
        {
            Shops.Remove(shop);
        }

        public void AddShoppingList(ShoppingList shList)
        {
            ShoppingLists.Add(shList);
        }

        private void GoToAddShopPage()
        {
            navigationService.NavigateTo("addShop");
        }

        private void GoToEditShopPage()
        {
            navigationService.NavigateTo("editShop", SelectedShop);
        }

        private async void GoToDeleteShop()
        {
            bool result = await dialogService.ShowMessage(
                ResourceLoader.GetForCurrentView().GetString("DeleteShopDialogContent"),
                ResourceLoader.GetForCurrentView().GetString("DeleteShopDialogTitle"),
                ResourceLoader.GetForCurrentView().GetString("DeleteShopDialogButtonYes"),
                ResourceLoader.GetForCurrentView().GetString("DeleteShopDialogButtonNo"),
                null);

            if (result)
            {
                DeleteShop(SelectedShop);
                SelectedShop = null;
            }
        }

        private void GoToDetailsShop()
        {
            
        }

        private void GoToAddShoppingListPage()
        {
            navigationService.NavigateTo("addShoppingList");
        }

        private void GoToShoppingListItem(ShoppingList shList)
        {
            navigationService.NavigateTo("addShoppingListItem", shList);
        }
    }
}
