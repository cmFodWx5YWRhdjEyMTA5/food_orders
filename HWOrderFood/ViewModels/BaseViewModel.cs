using System;
using System.Threading.Tasks;
using System.Windows.Input;
using HWOrderFood.Helpers;
using HWOrderFood.ModelLayers.Dish;
using HWOrderFood.Views;
using Prism.Mvvm;
using Prism.Navigation;

namespace HWOrderFood.ViewModels
{
    public class BaseViewModel : BindableBase
    {
        private readonly INavigationService _navigationService;

        public BaseViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        private ICommand _navigateMenuCommand;
        public ICommand NavigateMenuCommand
        {
            get
            {
                return _navigateMenuCommand ?? (_navigateMenuCommand = SingleExecutionCommand.FromFunc(OnMenuCommandAsyng));
            }
        }

        private ICommand _navigateDishCommand;
        public ICommand NavigateDishCommand
        {
            get { return _navigateDishCommand ?? (_navigateDishCommand = SingleExecutionCommand.FromFunc(OnDishCommandAsync)); }
        }

        protected async Task OnDishCommandAsync()
        {
            await _navigationService.NavigateAsync(nameof(DishView));
        }

        protected async Task OnMenuCommandAsyng()
        {
            await _navigationService.NavigateAsync(nameof(MenuView));
        }
    }
}
