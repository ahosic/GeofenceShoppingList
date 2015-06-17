using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Mutzl.MvvmLight;
using ShoppingListWPApp.Models;
using Microsoft.Practices.ServiceLocation;

namespace ShoppingListWPApp.ViewModels
{
    class AddShoppingListViewModel : ViewModelBase
    {
        #region *** private Members ***
        /// <summary>
        /// This <c>INavigationService</c>-Object is used for navigating between pages.
        /// </summary>
        private INavigationService navigationService;
        /// <summary>
        /// This <c>IDialogService</c>-Object is used for displaying Dialogs on the Device-Screen.
        /// </summary>
        private IDialogService dialogService;
        #endregion

        #region *** Properties ***

        public string ListName { get; set; }

        public Shop SelectedShop { get; set; }

        public ObservableCollection<Shop> Shops { get; set; }

        public ICommand CancelCommand { get; set; }
        public ICommand AcceptCommand { get; set; }

        #endregion

        public AddShoppingListViewModel(INavigationService navigationService, IDialogService dialogService)
        {
            this.navigationService = navigationService;
            this.dialogService = dialogService;

            ListName = string.Empty;
            Shops = ServiceLocator.Current.GetInstance<MainPageViewModel>().Shops;

            AcceptCommand = new DependentRelayCommand(Accept, isDataValid, this, () => ListName, () => SelectedShop);
            CancelCommand = new RelayCommand(Cancel);

            
            showDialogShop();
        }

        public void Accept()
        {
            ShoppingList shList = new ShoppingList(ListName,SelectedShop);
            ServiceLocator.Current.GetInstance<MainPageViewModel>().AddShoppingList(shList);
            ListName = string.Empty;
            navigationService.GoBack();
        }
        public void Cancel()
        {
            navigationService.GoBack();
        }
        public bool isDataValid()
        {
            return !string.IsNullOrWhiteSpace(ListName) && SelectedShop != null;
        }

        public async void showDialogShop()
        {
            if (Shops.Count == 0)
            {
                bool result = true;

                // Show dialog
                result = await dialogService.ShowMessage("You need to add a Shop",
                    "Add Shop",
                    "OK",
                    "Cancel",
                    null);

                // Check, if user pressed the "Proceed-Button"
                if (result)
                {   
                    navigationService.NavigateTo("addShop");
                }else
                {
                    navigationService.NavigateTo("main");
                }
            }
        }



    }
}
