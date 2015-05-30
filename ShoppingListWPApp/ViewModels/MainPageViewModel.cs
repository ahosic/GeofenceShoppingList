using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;

namespace ShoppingListWPApp.ViewModels
{
    class MainPageViewModel : ViewModelBase
    {
        #region *** Private Members ***

        private INavigationService navigationService;
        private IDialogService dialogService;

        #endregion

        #region *** Properties ***
        #endregion

        public MainPageViewModel(INavigationService navigationService, IDialogService dialogService)
        {
            // Services
            this.navigationService = navigationService;
            this.dialogService = new DialogService();
        }
    }
}
