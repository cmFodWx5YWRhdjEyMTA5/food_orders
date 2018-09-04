using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using HWOrderFood.Enums;
using HWOrderFood.Helpers;
using HWOrderFood.ModelLayers.Dish;
using HWOrderFood.ModelLayers.DishMenu.Requests;
using HWOrderFood.Services.Dish;
using HWOrderFood.Services.DishMenu;
using HWOrderFood.Views;
using Prism.Navigation;
using Xamarin.Forms;

namespace HWOrderFood.ViewModels
{
    public class DishViewModel : BaseViewModel, INavigatedAware
    {
        private readonly INavigationService _navigationService;
        private readonly IDishService _dishService;
        private readonly IUserDialogs _userDialog;
        private readonly IDishMenuService _dishMenuService;

        private const string DateMustNotBeLessToday = "Дата не должна быть раньше сегодня";
        private const string SelectPositions = "Выберете позиции";

        public DishViewModel(INavigationService navigationService,
                             IDishService dishService,
                             IUserDialogs userDialog,
                             IDishMenuService dishMenuService) : base(navigationService)
        {
            _navigationService = navigationService;
            _dishService = dishService;
            _userDialog = userDialog;
            _dishMenuService = dishMenuService;
            Date = DateTime.Now;
        }

        private DateTime _date;
        public DateTime Date
        {
            get { return _date; }
            set { SetProperty(ref _date, value); }
        }

        private ObservableCollection<DishModel> _garnishDishList = new ObservableCollection<DishModel>();
        public ObservableCollection<DishModel> GarnishDishList
        {
            get { return _garnishDishList; }
            set { SetProperty(ref _garnishDishList, value); }
        }

        private ObservableCollection<DishModel> _meatDishList;
        public ObservableCollection<DishModel> MeatDishList
        {
            get { return _meatDishList; }
            set { SetProperty(ref _meatDishList, value); }
        }

        private ObservableCollection<DishModel> _saladDishList;
        public ObservableCollection<DishModel> SaladDishList
        {
            get { return _saladDishList; }
            set { SetProperty(ref _saladDishList, value); }
        }

        private ObservableCollection<DishModel> _breadDishList;
        public ObservableCollection<DishModel> BreadDishList
        {
            get { return _breadDishList; }
            set { SetProperty(ref _breadDishList, value); }
        }

        private List<int> _dishIdList;
        public List<int> DishIdList
        {
            get { return _dishIdList; }
            set { SetProperty(ref _dishIdList, value); }
        }

        private ICommand _navigateAddDishCommand;
        public ICommand NavigateAddDishCommand
        {
            get
            {
                return _navigateAddDishCommand ?? (_navigateAddDishCommand = SingleExecutionCommand.FromFunc(OnAddDishCommandAsync));
            }
        }

        private ICommand _deleteGarnishDishCommand;
        public ICommand DeleteGarnishDishCommand
        {
            get { return _deleteGarnishDishCommand ?? (_deleteGarnishDishCommand = SingleExecutionCommand.FromFunc<int>(OnDeleteGarnishDishCommandAsync)); }
        }

        private ICommand _deleteMeatDishCommand;
        public ICommand DeleteMeatDishCommand
        {
            get { return _deleteMeatDishCommand ?? (_deleteMeatDishCommand = SingleExecutionCommand.FromFunc<int>(OnDeleteMeatDishCommandAsync)); }
        }

        private ICommand _deleteSaladDishCommand;
        public ICommand DeleteSaladDishCommand
        {
            get { return _deleteSaladDishCommand ?? (_deleteSaladDishCommand = SingleExecutionCommand.FromFunc<int>(OnDeleteSaladDishCommandAsync)); }
        }

        private ICommand _deleteBreadDishCommand;
        public ICommand DeleteBreadDishCommand
        {
            get { return _deleteBreadDishCommand ?? (_deleteBreadDishCommand = SingleExecutionCommand.FromFunc<int>(OnDeleteBreadDishCommandAsync)); }
        }

        private ICommand _saveMenuCommand;
        public ICommand SaveMenuCommad
        {
            get { return _saveMenuCommand ?? (_saveMenuCommand = SingleExecutionCommand.FromFunc(OnSaveMenuCommandAsync)); }
        }

        private async Task OnDeleteGarnishDishCommandAsync(int id)
        {
            var isDeleted = await DeleteDishAsync(id);
            if(isDeleted)
            {
                var garnish = GarnishDishList.FirstOrDefault(x => x.Id == id);
                if (garnish != null)
                    GarnishDishList.Remove(garnish);
            }
        }

        private async Task OnDeleteMeatDishCommandAsync(int id)
        {
            var isDeleted = await DeleteDishAsync(id);
            if (isDeleted)
            {
                var meat = MeatDishList.FirstOrDefault(x => x.Id == id);
                if (meat != null)
                    MeatDishList.Remove(meat);
            }
        }

        private async Task OnDeleteSaladDishCommandAsync(int id)
        {
            var isDeleted = await DeleteDishAsync(id);
            if (isDeleted)
            {
                var salad = SaladDishList.FirstOrDefault(x => x.Id == id);
                if (salad != null)
                    SaladDishList.Remove(salad);
            }
        }

        private async Task OnDeleteBreadDishCommandAsync(int id)
        {
            var isDeleted = await DeleteDishAsync(id);
            if (isDeleted)
            {
                var bread = BreadDishList.FirstOrDefault(x => x.Id == id);
                if (bread != null)
                    BreadDishList.Remove(bread);
            }
        }

        private async Task OnAddDishCommandAsync()
        {
            await _navigationService.NavigateAsync(nameof(AddDishView));
        }


        private async Task OnSaveMenuCommandAsync()
        {
            if (_date == null || (_date != null && _date.Date < DateTime.Now))
                await _userDialog.AlertAsync(DateMustNotBeLessToday);
            else
            {
                List<int> dishIdList = new List<int>();
                dishIdList.AddRange(GarnishDishList.Where(x => x.IsSelected).Select(x=>x.Id));
                dishIdList.AddRange(MeatDishList.Where(x => x.IsSelected).Select(x => x.Id));
                dishIdList.AddRange(SaladDishList.Where(x => x.IsSelected).Select(x => x.Id));
                dishIdList.AddRange(BreadDishList.Where(x => x.IsSelected).Select(x => x.Id));
                if(dishIdList.Any())
                {
                    DishMenuRequest dishMenuRequest = new DishMenuRequest()
                    {
                        Date = _date,
                        DishIds = dishIdList
                    };
                    var result = await _dishMenuService.CreateAsync(dishMenuRequest);
                    if (result.IsSuccess)
                        await OnMenuCommandAsyng();
                    else
                        await _userDialog.AlertAsync(Constants.ErrorWithSendDataToServer);
                }
                else
                {
                    await _userDialog.AlertAsync(SelectPositions);
                }
            }
        }

        private async void OnChangeCheckboxCommandAsync(DishModel model)
        {
            model.IsSelected = !model.IsSelected;
        }

        /// <summary>
        /// Call by navigate from page
        /// </summary>
        /// <param name="parameters">Parameters.</param>
        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            return;
        }

        /// <summary>
        /// Call by navigate to page
        /// </summary>
        /// <param name="parameters">Parameters.</param>
        public async void OnNavigatedTo(NavigationParameters parameters)
        {
            var result = await _dishService.GetAsync();
            if (result.IsSuccess)
            {
                var dishModels = result.Result.ToList();

                GarnishDishList = new ObservableCollection<DishModel>(dishModels.Where(x => x.CategoryType == DishCategory.Garnish).ToList());
                MeatDishList = new ObservableCollection<DishModel>(dishModels.Where(x => x.CategoryType == DishCategory.Meat).ToList());

                SaladDishList = new ObservableCollection<DishModel>(dishModels.Where(x => x.CategoryType == DishCategory.Salad).ToList());
                BreadDishList = new ObservableCollection<DishModel>(dishModels.Where(x => x.CategoryType == DishCategory.Bread).ToList());

                foreach (var dish in dishModels)
                {
                    dish.ChangeCheckboxCommand = new Command<DishModel>(OnChangeCheckboxCommandAsync);
                }
            }
        }

        private async Task<bool> DeleteDishAsync(int id)
        {
            var isDeleted = false;
            var result = await _dishService.DeleteAsync(id);
            if(result != null && result.IsSuccess)
            {
                isDeleted = true;
            }
            else
            {
                await _userDialog.AlertAsync(Constants.ErrorWithSendDataToServer);
            }
            return isDeleted;
        }
    }
}
