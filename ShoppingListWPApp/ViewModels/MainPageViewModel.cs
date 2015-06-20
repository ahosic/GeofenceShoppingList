﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Windows.ApplicationModel.Resources;
using Windows.Devices.Geolocation;
using Windows.Storage;
using Windows.Storage.Streams;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using Newtonsoft.Json;
using ShoppingListWPApp.Common;
using ShoppingListWPApp.Models;

namespace ShoppingListWPApp.ViewModels
{
    /// <summary>
    /// This is the ViewModel for the <c>MainPage</c>-View.
    /// </summary>
    class MainPageViewModel : ViewModelBase
    {
        #region *** Private Members ***

        /// <summary>
        /// This <c>INavigationService</c>-Object is used for navigating between pages.
        /// </summary>
        private INavigationService navigationService;
        /// <summary>
        /// This <c>IDialogService</c>-Object is used for displaying Dialogs on the Device-Screen.
        /// </summary>
        private IDialogService dialogService;
        /// <summary>
        /// This <c>Geolocator</c>-Object is used for retrieving the geographical position of the device using GPS.
        /// </summary>
        private Geolocator locator;

        private string shopsFilename;

        #endregion

        #region *** Properties ***

        /// <summary>
        /// Holds all <c>Shop</c>-Objects and notifies Views through Data Binding when Changes occur.
        /// </summary>
        public ObservableCollection<Shop> Shops { get; set; }
        /// <summary>
        /// Holds all <c>ShoppingList</c>-Objects and notifies Views through Data Binding when Changes occur.
        /// </summary>
        public ObservableCollection<ShoppingList> ShoppingLists { get; set; }
        /// <summary>
        /// The currently selected <c>Shop</c>-Object in the Shops-<c>ListView</c> of the <c>MainPage</c>-View.
        /// </summary>
        public Shop SelectedShop { get; set; }
        /// <summary>
        /// The currently selected <c>ShoppingList</c>-Object in the ShoppingLists-<c>ListView</c> of the <c>MainPage</c>-View.
        /// </summary>
        public ShoppingList SelectedShoppingList { get; set; }
        /// <summary>
        /// Gets or Sets the Command that is issued by the user, in order to add a new Shop.
        /// </summary>
        public ICommand AddShopCommand { get; set; }
        /// <summary>
        /// Gets or Sets the Command that is issued by the user, in order to display details of a previously selected Shop.
        /// </summary>
        public ICommand DetailsShopCommand { get; set; }
        /// <summary>
        /// Gets or Sets the Command that is issued by the user, in order to modify a previously selected Shop.
        /// </summary>
        public ICommand EditShopCommand { get; set; }
        /// <summary>
        /// Gets or Sets the Command that is issued by the user, in order to delete a previously selected Shop.
        /// </summary>
        public ICommand DeleteShopCommand { get; set; }
        /// <summary>
        /// Gets or Sets the Command that is issued by the user, in order to add a new ShoppingList.
        /// </summary>
        public ICommand AddShoppingListCommand { get; set; }
        /// <summary>
        /// Gets or Sets the Command that is issued by the user, in order to add a new ShoppingListItem.
        /// </summary>
        public ICommand AddShoppingListItemCommand { get; set; }
        /// <summary>
        /// Gets or Sets the Command that is issued by the user, in order to delete a previously selected ShoppingList.
        /// </summary>
        public ICommand DeleteShoppingListCommand { get; set; }

        public ICommand EditShoppingListCommand { get; set; }

        #endregion

        public MainPageViewModel(INavigationService navigationService, IDialogService dialogService, Geolocator locator)
        {
            // Services
            this.navigationService = navigationService;
            this.dialogService = dialogService;
            this.locator = locator;

            // Initialize Observable Collections
            ShoppingLists = new ObservableCollection<ShoppingList>();

            // Commands
            AddShopCommand = new RelayCommand(GoToAddShopPage);
            EditShopCommand = new RelayCommand(GoToEditShopPage);
            DeleteShopCommand = new RelayCommand(GoToDeleteShop);
            DetailsShopCommand = new RelayCommand(GoToDetailsShop);
            AddShoppingListCommand = new RelayCommand(GoToAddShoppingListPage);
            DeleteShoppingListCommand = new RelayCommand(GoToDeleteShoppingList);

            // Load data
            shopsFilename = "shops.json";
            LoadShops();
        }

        #region *** Shop ***

        /// <summary>
        /// Adds a new <c>Shop</c>-Object to the <c>Shops</c>-Collection.
        /// </summary>
        /// <param name="shop">The <c>Shop</c>-Object that should added to the <c>Shops</c>-Collection.</param>
        public void AddShop(Shop shop)
        {
            // Add Shop to collection
            Shops.Add(shop);

            // Save shops to isolated storage
            SaveShops();

            // Notify for change
            this.RaisePropertyChanged(() => Shops);

            // Create new Geofence
            ServiceLocator.Current.GetInstance<GeoHelper>().AddGeofence(shop);
        }

        /// <summary>
        /// Replaces an old <c>Shop</c>-Object with a modified one.
        /// </summary>
        /// <param name="oldShop">The <c>Shop</c>-Object that should be replaced.</param>
        /// <param name="newShop">The <c>Shop</c>-Object that should be inserted into the Collection.</param>
        public void EditShop(Shop oldShop, Shop newShop)
        {
            // Get index of old object
            int idx = Shops.IndexOf(oldShop);

            // Remove old object and insert new object at the same position as the old one
            Shops.Remove(oldShop);
            Shops.Insert(idx, newShop);

            // Save shops to isolated storage
            SaveShops();

            // Replace old Geofence with new one
            ServiceLocator.Current.GetInstance<GeoHelper>().ModifyGeofence(oldShop.ID, newShop);
        }

        /// <summary>
        /// Removes a <c>Shop</c>-Object from the <c>Shops</c>-Collection.
        /// </summary>
        /// <param name="shop">The <c>Shop</c>-Object that should be removed from the Collection.</param>
        public void DeleteShop(Shop shop)
        {
            Shops.Remove(shop);

            // Save shops to isolated storage
            SaveShops();

            // Remove Geofence
            ServiceLocator.Current.GetInstance<GeoHelper>().RemoveGeofence(shop.ID);
        }

        /// <summary>
        /// Gets the Index of a <c>Shop</c>-Object that is element of the <c>Shops</c>-Collection.
        /// </summary>
        /// <param name="shop">The <c>Shop</c>-Object, for which an index should be retrieved.</param>
        /// <returns>The Index of the passed <c>Shop</c>-Object.</returns>
        public int IndexOfShop(Shop shop)
        {
            return Shops.IndexOf(shop);
        }

        /// <summary>
        /// Gets a <c>Shop</c>-Object at a specified Index out of the <c>Shops</c>-Collection.
        /// </summary>
        /// <param name="index">The Index of the <c>Shop</c>-Object in the <c>Shops</c>-Collection.</param>
        /// <returns>The <c>Shop</c>-Object at the specified Index in the <c>Shops</c>-Collection.</returns>
        public Shop GetShopByIndex(int index)
        {
            return Shops.ElementAt(index);
        }

        #endregion

        #region *** ShoppingList ***

        /// <summary>
        /// Adds a new <c>ShoppingList</c>-Object to the <c>ShoppingLists</c>-Collection.
        /// </summary>
        /// <param name="shoppingList">The <c>ShoppingList</c>-Object that should added to the <c>ShoppingLists</c>-Collection.</param>
        public void AddShoppingList(ShoppingList shoppingList)
        {
            ShoppingLists.Add(shoppingList);
        }

        /// <summary>
        /// Removes a <c>ShoppingList</c>-Object from the <c>ShoppingLists</c>-Collection.
        /// </summary>
        /// <param name="shoppingList">The <c>ShoppingList</c>-Object that should be removed from the Collection.</param>
        public void DeleteShoppingList(ShoppingList shoppingList)
        {
            ShoppingLists.Remove(shoppingList);
        }

        public void EditShoppingList()
        {

        }

        #endregion

        #region *** Command methods ***

        /// <summary>
        /// Navigates the User to the <c>AddShop</c>-View.
        /// </summary>
        private void GoToAddShopPage()
        {
            navigationService.NavigateTo("addShop");
        }

        /// <summary>
        /// Navigates the User to the <c>EditShop</c>-View.
        /// </summary>
        private void GoToEditShopPage()
        {
            navigationService.NavigateTo("editShop", SelectedShop);
        }

        /// <summary>
        /// After opening a dialog and asking for confirmation it removes the selected <c>Shop</c>-Object out of the <c>Shops</c>-Collection.
        /// </summary>
        private async void GoToDeleteShop()
        {
            // Show dialog
            bool result = await dialogService.ShowMessage(
                ResourceLoader.GetForCurrentView().GetString("DeleteShopDialogContent"),
                ResourceLoader.GetForCurrentView().GetString("DeleteShopDialogTitle"),
                ResourceLoader.GetForCurrentView().GetString("DeleteShopDialogButtonYes"),
                ResourceLoader.GetForCurrentView().GetString("DeleteShopDialogButtonNo"),
                null);

            // Check, if user pressed the "Proceed-Button"
            if (result)
            {
                // Delete selected Shop object
                DeleteShop(SelectedShop);
                SelectedShop = null;
            }
        }

        /// <summary>
        /// Navigates the User to the <c>DetailsShop</c>-View.
        /// </summary>
        private void GoToDetailsShop()
        {
            navigationService.NavigateTo("detailsShop", this.IndexOfShop(SelectedShop));
        }

        /// <summary>
        /// Navigates the User to the <c>AddShoppingList</c>-View.
        /// </summary>
        private void GoToAddShoppingListPage()
        {
            ShowDialogShop();
            navigationService.NavigateTo("addShoppingList");
        }

        /// <summary>
        /// After opening a dialog and asking for confirmation it removes the selected <c>ShoppingList</c>-Object out of the <c>ShoppingLists</c>-Collection.
        /// </summary>
        private async void GoToDeleteShoppingList()
        {
            // Show dialog
            bool result = await dialogService.ShowMessage(
                ResourceLoader.GetForCurrentView().GetString("DeleteShoppingListDialogContent"),
                ResourceLoader.GetForCurrentView().GetString("DeleteShoppingListDialogTitle"),
                ResourceLoader.GetForCurrentView().GetString("DeleteShoppingListDialogButtonYes"),
                ResourceLoader.GetForCurrentView().GetString("DeleteShoppingListDialogButtonNo"),
                null);

            // Check, if user pressed the "Proceed-Button"
            if (result)
            {
                // Delete selected ShoppingList object
                DeleteShoppingList(SelectedShoppingList);
                SelectedShoppingList = null;
            }
        }

        /// <summary>
        /// Checks, if Shops-Collection has items
        /// </summary>
        public async void ShowDialogShop()
        {
            if (ServiceLocator.Current.GetInstance<AddShoppingListViewModel>().Shops.Count == 0)
            {
                bool result = true;
                result = await dialogService.ShowMessage(
                    ResourceLoader.GetForCurrentView().GetString("AddShopDialogContent"),
                    ResourceLoader.GetForCurrentView().GetString("AddShopDialogTitle"),
                    ResourceLoader.GetForCurrentView().GetString("AddShopDialogButtonYes"),
                    ResourceLoader.GetForCurrentView().GetString("AddShopDialogButtonNo"),
                    null);

                // Check, if user pressed the "Yes-Button"
                if (result)
                {
                    navigationService.NavigateTo("addShop");
                }
                else
                {
                    navigationService.NavigateTo("main");
                }
            }

        }

        #endregion

        #region *** Input/Output ***

        /// <summary>
        /// Saves all created shops in the isolated storage of the device as JSON data.
        /// </summary>
        private async void SaveShops()
        {
            try
            {
                // Get App's folder and create file
                var folder = ApplicationData.Current.LocalFolder;
                var file = await folder.CreateFileAsync(shopsFilename, CreationCollisionOption.ReplaceExisting);

                // Write Shops to file as JSON data
                await FileIO.WriteTextAsync(file, JsonConvert.SerializeObject(Shops), UnicodeEncoding.Utf8);
            }
            catch (Exception ex)
            {
                dialogService.ShowMessage("An error occured while saving Shop data.", "Error");
            }
        }

        /// <summary>
        /// Loads all created shops out of the isolated storage of the device.
        /// </summary>
        private async void LoadShops()
        {
            try
            {
                // Get App's folder and create file
                var folder = ApplicationData.Current.LocalFolder;
                var file = await folder.GetFileAsync(shopsFilename);

                // Load data out of file
                string data = await FileIO.ReadTextAsync(file, UnicodeEncoding.Utf8);
                Shops = JsonConvert.DeserializeObject<ObservableCollection<Shop>>(data) ??
                        new ObservableCollection<Shop>();
            }
            catch (Exception ex)
            {
                // Initialize collection
                Shops = new ObservableCollection<Shop>();
            }
        }

        #endregion
    }
}
