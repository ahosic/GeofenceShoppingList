using System.Collections.ObjectModel;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using Mutzl.MvvmLight;
using ShoppingListWPApp.Models;

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
            AddItemCommand = new DependentRelayCommand(CreateItem, IsDataValid, this, () => Name, () => AmountAndMeasure);
            DeleteItemCommand = new RelayCommand(DeleteItem);

            // Initialize all Fields with standard values
            InitializeFields();
        }

        #region *** command methods ***

        /// <summary>
        /// Creates a new <c>ShoppingListItem</c>-Object, adds it to the <c>ShoppingLists</c>-Collection (located in the <c>MainPageViewModel</c>).
        /// </summary>
        private void CreateItem()
        {
            ShoppingListItem item = new ShoppingListItem(Name, AmountAndMeasure);
            ShoppingList.AddItem(item);

            // Get old object
            ShoppingList oldShoppingList = ServiceLocator.Current.GetInstance<MainPageViewModel>().GetShoppingListByID(ShoppingList.ID);

            // Update Shoppinglist
            ServiceLocator.Current.GetInstance<MainPageViewModel>().EditShoppingList(oldShoppingList, ShoppingList);

            // Reset all fields
            InitializeFields();
        }

        /// <summary>
        /// Removes a <c>ShoppingListItem</c>-Object from the <c>Items</c>-Collection.
        /// </summary>
        private void DeleteItem()
        {
            Items.Remove(ShoppingListItem);

            // Get old object
            ShoppingList oldShoppingList = ServiceLocator.Current.GetInstance<MainPageViewModel>().GetShoppingListByID(ShoppingList.ID);

            // Update Shoppinglist
            ServiceLocator.Current.GetInstance<MainPageViewModel>().EditShoppingList(oldShoppingList, ShoppingList);

            ShoppingListItem = null;
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

        /// <summary>
        /// Checks, if all required values are set and valid.
        /// </summary>
        /// <returns>Returns <c>true</c> if all inputted values are valid, <c>false</c> if the provided data is invalid.</returns>
        private bool IsDataValid()
        {
            if (Name.Trim().Equals(string.Empty) || AmountAndMeasure.Trim().Equals(string.Empty))
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}
