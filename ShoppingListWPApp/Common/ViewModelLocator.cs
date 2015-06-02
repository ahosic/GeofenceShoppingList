using Windows.Devices.Geolocation;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using ShoppingListWPApp.ViewModels;
using ShoppingListWPApp.Views;

namespace ShoppingListWPApp.Common
{
    /// <summary>
    /// Use this class to register all used ViewModels and Services (e. g. NavigationService, IDialogService...)
    /// </summary>
    class ViewModelLocator
    {
        #region *** ViewModels ***

        public MainPageViewModel MainPage
        {
            get { return ServiceLocator.Current.GetInstance<MainPageViewModel>(); }
        }

        public AddShopViewModel AddShop
        {
            get { return ServiceLocator.Current.GetInstance<AddShopViewModel>(); }
        }

        public EditShopViewModel EditShop
        {
            get { return ServiceLocator.Current.GetInstance<EditShopViewModel>(); }
        }

        #endregion

        static ViewModelLocator()
        {
            // Create IoC container
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            // Setup a Navigation service
            NavigationService navigationService = new NavigationService();
            navigationService.Configure("main", typeof(MainPage));
            navigationService.Configure("addShop", typeof(AddShop));
            navigationService.Configure("editShop", typeof(EditShop));

            // Register services
            SimpleIoc.Default.Register<INavigationService>(() => navigationService);
            SimpleIoc.Default.Register<IDialogService>(() => new DialogService());
            SimpleIoc.Default.Register<Geolocator>();

            // Register ViewModels
            SimpleIoc.Default.Register<MainPageViewModel>();
            SimpleIoc.Default.Register<AddShopViewModel>();
            SimpleIoc.Default.Register<EditShopViewModel>();
        }
    }
}
