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
        public string AmountAndMeasure { get; set; }
        public ObservableCollection<ShoppingListItem> Items { get; set; }
        public ICommand AddItemCommand { get; set; }

        #endregion

        public AddShoppingListItemViewModel(INavigationService navigationService) 
        {
            this.navigationService = navigationService;

            Name = string.Empty;
            AmountAndMeasure = string.Empty;

            AddItemCommand = new RelayCommand(CreateItem);

        }
        public void CreateItem()
        {
            ShoppingListItem item = new ShoppingListItem(Name, AmountAndMeasure);
            AddItemToShoppingList(item);

            Name = string.Empty;
            AmountAndMeasure = string.Empty;
            
        }

        public void AddItemToShoppingList(ShoppingListItem shoppingListItem)
        {
            ShoppingList.AddItem(shoppingListItem);
        }


    }
}
