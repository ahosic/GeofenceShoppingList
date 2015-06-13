using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using ShoppingListWPApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ShoppingListWPApp.ViewModels
{
    class AddShoppingListItemViewModel : ViewModelBase
    {
        #region *** Private Members ***
        private INavigationService navigationService;
        #endregion

        #region *** Properties ***

        public ShoppingList ShoppingList { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }
        public string Measure { get; set; }
        public ObservableCollection<ShoppingListItem> Items { get; set; }
        public ICommand AddItemCommand { get; set; }

        #endregion

        public AddShoppingListItemViewModel(INavigationService navigationService) 
        {
            this.navigationService = navigationService;

            Name = string.Empty;
            Amount = 1;

            AddItemCommand = new RelayCommand(CreateItem);

        }
        public void CreateItem()
        {
            ShoppingListItem item = new ShoppingListItem(Name, Amount, Measure);
            AddItemToShoppingList(item);

            Name = string.Empty;
            Amount = 1;
            
        }

        public void AddItemToShoppingList(ShoppingListItem shoppingListItem)
        {
            ShoppingList.AddItem(shoppingListItem);
        }


    }
}
