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
        private INavigationService navigationService;
        #endregion

        #region *** Properties ***

        public string ListName { get; set; }

        public string ShopName { get; set; }

        public ObservableCollection<Shop> Shops { get; set; }

        public ICommand CancelCommand { get; set; }
        public ICommand AcceptCommand { get; set; }

        #endregion

        public AddShoppingListViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;

            ListName = string.Empty;
            Shops = ServiceLocator.Current.GetInstance<MainPageViewModel>().Shops;

            AcceptCommand = new DependentRelayCommand(Accept, isDataValid, this, () => ListName);
            CancelCommand = new RelayCommand(Cancel);

        }

        public void Accept()
        {
            ShoppingList shList = new ShoppingList(ListName,ShopName);
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
            return !string.IsNullOrWhiteSpace(ListName);
        }



    }
}
