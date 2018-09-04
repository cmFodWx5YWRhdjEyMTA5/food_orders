using System;
using System.Threading.Tasks;
using System.Windows.Input;
using HWOrderFood.Helpers;
using HWOrderFood.Views;
using Prism.Navigation;

namespace HWOrderFood.ViewModels
{
    public class MenuViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;

        public MenuViewModel(INavigationService navigationService) : base(navigationService)
        {
            _navigationService = navigationService;
        }

        private ICommand _navigateCreateOrderCommand;
        public ICommand NavigateCreateOrderCommand
        {
            get { return _navigateCreateOrderCommand ?? (_navigateCreateOrderCommand = SingleExecutionCommand.FromFunc(OnNavigateCreateOrderCommandAsync)); }
        }

        private async Task OnNavigateCreateOrderCommandAsync()
        {
            
            await _navigationService.NavigateAsync(nameof(AddOrderView));
        }
    }
}
