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
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Resources;
using Windows.UI.Core;

namespace ShoppingListWPApp.ViewModels
{
    /// <summary>
    /// This is the ViewModel for the <c>AddShoppingListItem</c>-View.
    /// </summary>
    class AddShoppingListItemViewModel : ViewModelBase
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

        #endregion

        #region *** Properties ***

        /// <summary>
        /// Gets or Sets the Name of the new ShoppingListItem.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets or Sets the AmountAndMeasure of the new ShoppingListItem.
        /// </summary>
        public string AmountAndMeasure { get; set; }
        /// <summary>
        /// Gets or Sets the ShoppingList of the new ShoppingListItem.
        /// </summary>
        public ShoppingList ShoppingList { get; set; }
        /// <summary>
        /// Gets or Sets the ShoppingListItem for delete.
        /// </summary>
        public ShoppingListItem ShoppingListItem { get; set; }
        /// <summary>
        /// Holds all <c>ShoppingListItem</c>-Objects and notifies Views through Data Binding when Changes occur.
        /// </summary>
        public ObservableCollection<ShoppingListItem> Items { get; set; }
        /// <summary>
        /// Gets or Sets the Command that is issued by the user, in order to finish the creation-process of a new ShoppingListItem.
        /// </summary>
        public ICommand AddItemCommand { get; set; }
        /// <summary>
        /// Gets or Sets the Command that is issued by the user, in order to finish the delete-process of a ShoppingListItem.
        /// </summary>
        public ICommand DeleteItemCommand { get; set; }

        #endregion

        public AddShoppingListItemViewModel(INavigationService navigationService, IDialogService dialogService) 
        {
            // Services
            this.navigationService = navigationService;
            this.dialogService = dialogService;

            // Commands
            AddItemCommand = new RelayCommand(CreateItem);
            DeleteItemCommand = new RelayCommand(GoToDeleteShoppingListItem);

            // Initialize all Fields with standard values
            InitializeFields();

        }

        #region *** command methods ***

        /// <summary>
        /// Creates a new <c>ShoppingListItem</c>-Object, adds it to the <c>ShoppingLists</c>-Collection (located in the <c>MainPageViewModel</c>).
        /// </summary>
        public async void CreateItem()
        {
            if(!string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(AmountAndMeasure))
            {
                ShoppingListItem item = new ShoppingListItem(Name, AmountAndMeasure);
                ShoppingList.AddItem(item);

                InitializeFields();
            }
            
        }

        /// <summary>
        /// Removes a <c>ShoppingListItem</c>-Object from the <c>Items</c>-Collection.
        /// </summary>
        /// <param name="shoppingListItem">The <c>ShoppingListItem</c>-Object that should be removed from the Collection.</param>
        public void DeleteItem(ShoppingListItem shoppingListItem)
        {
            Items.Remove(shoppingListItem);
        }

        /// <summary>
        /// After opening a dialog and asking for confirmation it removes the selected <c>ShoppingListItem</c>-Object out of the <c>Items</c>-Collection.
        /// </summary>
        private async void GoToDeleteShoppingListItem()
        {
            // Show dialog
            bool result = await dialogService.ShowMessage(
                ResourceLoader.GetForCurrentView().GetString("DeleteShoppingListItemDialogContent"),
                ResourceLoader.GetForCurrentView().GetString("DeleteShoppingListItemDialogTitle"),
                ResourceLoader.GetForCurrentView().GetString("DeleteShoppingListItemDialogButtonYes"),
                ResourceLoader.GetForCurrentView().GetString("DeleteShoppingListItemDialogButtonNo"),
                null);

            // Check, if user pressed the "Proceed-Button"
            if (result)
            {
                // Delete selected ShoppingListItem object
                DeleteItem(ShoppingListItem);
                ShoppingListItem = null;
            }
        }
        #endregion

        #region *** private methods ***

        /// <summary>
        /// Initializes all Fields with standard values.
        /// </summary>
        private void InitializeFields()
        {
            Name = string.Empty;
            AmountAndMeasure = string.Empty;
        }

        #endregion
    }
}
