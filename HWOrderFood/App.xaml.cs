using System;
using HWOrderFood.Views;
using Prism.Unity;
using Prism.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Prism;
using Prism.Ioc;
using Acr.UserDialogs;
using HWOrderFood.Services.DiskCache;
using HWOrderFood.Services.Json;
using HWOrderFood.Services.Rest;
using HWOrderFood.Services.Dish;
using HWOrderFood.Services.Analitics;
using HWOrderFood.Services.DishMenu;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace HWOrderFood
{
    public partial class App : PrismApplication
    {
        public static T Resolve<T>()
        {
            return (Current as App).Container.Resolve<T>();
        }

		public App() : this(null)
        {
            
        }

        public App(IPlatformInitializer initializer = null) : base(initializer)
        {
			MainPage = new NavigationPage(new MenuView());
        }


        #region -- Overrides --

        protected override void OnInitialized()
        {
            InitializeComponent();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterInstance<IUserDialogs>(UserDialogs.Instance);
            containerRegistry.RegisterInstance<IDiskCacheService>(Container.Resolve<DiskCacheService>());
            containerRegistry.RegisterInstance<IJsonService>(Container.Resolve<JsonService>());
            containerRegistry.RegisterInstance<IRestService>(Container.Resolve<RestService>());
            containerRegistry.RegisterInstance<IBaseRestService>(Container.Resolve<BaseRestService>());
            containerRegistry.RegisterInstance<IDishService>(Container.Resolve<DishService>());
            containerRegistry.RegisterInstance<IDishMenuService>(Container.Resolve<DishMenuService>());

            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MenuView>();
            containerRegistry.RegisterForNavigation<DishView>();
            containerRegistry.RegisterForNavigation<AddDishView>();
            containerRegistry.RegisterForNavigation<AddOrderView>();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        #endregion
    }
}
