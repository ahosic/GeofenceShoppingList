using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ShoppingListWPApp.ViewModels
{
    class AddShoppingListViewModel : ViewModelBase
    {
        #region *** private Members ***
        private INavigationService navigationService;
        #endregion

        #region *** Properties ***

        public string Name { get; set; }

        public ICommand CancelCommand { get; set; }

        #endregion

        public AddShoppingListViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;

            Name = string.Empty;

            CancelCommand = new RelayCommand(Cancel);
            
        }

        public void Cancel()
        {
            navigationService.GoBack();
        }



    }
}
