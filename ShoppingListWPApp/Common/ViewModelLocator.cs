using Windows.Devices.Geolocation;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using ShoppingListWPApp.ViewModels;
using ShoppingListWPApp.Views;

namespace ShoppingListWPApp.Common
{
    /// <summary>
    /// The <c>ViewModelLocator</c> registers all used ViewModels and Services (e. g. NavigationService, IDialogService...)
    /// </summary>
    class ViewModelLocator
    {
        #region *** ViewModels ***

        /// <summary>
        /// ViewModel for the <c>MainPage</c>-View.
        /// </summary>
        public MainPageViewModel MainPage
        {
            get { return ServiceLocator.Current.GetInstance<MainPageViewModel>(); }
        }
        /// <summary>
        /// ViewModel for the <c>AddShop</c>-View.
        /// </summary>
        public AddShopViewModel AddShop
        {
            get { return ServiceLocator.Current.GetInstance<AddShopViewModel>(); }
        }
        /// <summary>
        /// ViewModel for the <c>EditShop</c>-View.
        /// </summary>
        public EditShopViewModel EditShop
        {
            get { return ServiceLocator.Current.GetInstance<EditShopViewModel>(); }
        }
        /// <summary>
        /// ViewModel for the <c>DetailsShop</c>-View.
        /// </summary>
        public DetailsShopViewModel DetailsShop
        {
            get { return ServiceLocator.Current.GetInstance<DetailsShopViewModel>(); }
        }

        #endregion

        static ViewModelLocator()
        {
            // Create IoC container
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            // Configure the Navigation Service
            NavigationService navigationService = new NavigationService();
            navigationService.Configure("main", typeof(MainPage));
            navigationService.Configure("addShop", typeof(AddShop));
            navigationService.Configure("editShop", typeof(EditShop));
            navigationService.Configure("detailsShop", typeof(DetailsShop));

            // Register Services
            SimpleIoc.Default.Register<INavigationService>(() => navigationService);
            SimpleIoc.Default.Register<IDialogService>(() => new DialogService());
            SimpleIoc.Default.Register<Geolocator>();

            // Register ViewModels
            SimpleIoc.Default.Register<MainPageViewModel>();
            SimpleIoc.Default.Register<AddShopViewModel>();
            SimpleIoc.Default.Register<EditShopViewModel>();
            SimpleIoc.Default.Register<DetailsShopViewModel>();
        }
    }
}
