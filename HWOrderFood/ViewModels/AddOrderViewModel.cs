using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using HWOrderFood.Enums;
using HWOrderFood.Helpers;
using HWOrderFood.ModelLayers.Dish;
using HWOrderFood.ModelLayers.OrderDish.Models;
using HWOrderFood.ModelLayers.Portition.Models;
using HWOrderFood.Services.DishMenu;
using Prism.Navigation;
using Xamarin.Forms;

namespace HWOrderFood.ViewModels
{
    public class AddOrderViewModel : BaseViewModel, INavigatedAware
    {
        private readonly INavigationService _navigationService;
        private readonly IDishMenuService _dishMenuService;
        private readonly IUserDialogs _userDialog;

        private const string YouCanSelectOnlyOneGarnishInThisPortition = "Вы можете выбрать только один гарнир в этой порции";
        private const string YouCanSelectOnlyOneMeatInThisPortition = "Вы можете выбрать только одно мясное блюдо в этой порции";
        private const string YouCanSelectOnlyOneSaladInThisPortition = "Вы можете выбрать только один салат и один гарнир либо два салата и без гарнира в этой порции";
        private const string YouCanSelectOnlyOneBreadInThisPortition = "Вы можете выбрать только один хлеб в этой порции";
        private const string YouCanNotCreateNewPortion = "Вы не можете создать новую порцию пока не запоните текущую";

        private const string DuplicatePrefix = "2x ";
        private const string PricePostFix = " грн";

        public AddOrderViewModel(INavigationService navigationService,
                                 IDishMenuService dishMenuService,
                                 IUserDialogs userDialogs) : base(navigationService)
        {
            _navigationService = navigationService;
            _dishMenuService = dishMenuService;
            _userDialog = userDialogs;
        }

        private ICommand _addPortionCommand;
        public ICommand AddPortionCommand
        {
            get { return _addPortionCommand ?? (_addPortionCommand = SingleExecutionCommand.FromFunc(OnAddPortionAsync)); }
        }

        private DateTime _date;
        public DateTime Date
        {
            get { return _date; }
            set { SetProperty(ref _date, value); }
        }

        private IEnumerable<DishModel> _dishes { get; set; }

        private ObservableCollection<PortionModel> _portionModels;
        public ObservableCollection<PortionModel> PortionModels
        {
            get { return _portionModels; }
            set { SetProperty(ref _portionModels, value); }
        }

        private decimal _totalPrise { get; set; }

        private string _totalPriceStr;
        public string TotalPriceStr
        {
            get { return _totalPriceStr; }
            set 
            {
                SetProperty(ref _totalPriceStr, value); 
            }
        }

        private async void OnChangeCheckboxIsDuplicateCommandAsync(PortionModel model)
        {
            model.IsDuplicate = !model.IsDuplicate;
            if (model.IsDuplicate)
            {
                model.PortionInfo.GarnishName = model.PortionInfo.IsGarnishVisible ? DuplicatePrefix + model.PortionInfo.GarnishName : string.Empty;
                model.PortionInfo.MeatName = model.PortionInfo.IsMeatVisible ? DuplicatePrefix + model.PortionInfo.MeatName : string.Empty;
                model.PortionInfo.FirstSaladName = model.PortionInfo.IsFirstSaladVisible ? DuplicatePrefix + model.PortionInfo.FirstSaladName : string.Empty;
                model.PortionInfo.SecondSaladName = model.PortionInfo.IsSecondSaladVisible ? DuplicatePrefix + model.PortionInfo.SecondSaladName : string.Empty;
                model.PortionInfo.BreadName = model.PortionInfo.IsBreadVisible ? DuplicatePrefix + model.PortionInfo.BreadName : string.Empty;
            }
            else
            {
                model.PortionInfo.GarnishName = model.PortionInfo.GarnishName.Replace(DuplicatePrefix, "");
                model.PortionInfo.MeatName = model.PortionInfo.MeatName.Replace(DuplicatePrefix, "");
                model.PortionInfo.FirstSaladName = model.PortionInfo.FirstSaladName.Replace(DuplicatePrefix, "");
                model.PortionInfo.BreadName = model.PortionInfo.BreadName.Replace(DuplicatePrefix, "");
            }
            CalculateTotolPrice();
        }

        private async void OnChangeVisiblePortitionCommandAsync(PortionModel model)
        {
            model.IsVisible = !model.IsVisible;
        }

        private async void OnChangeCheckboxGarnishCommandAsync(PortionWithCheckDish model)
        {
            if (model.Portion.SaladList.Where(x => x.IsSelected).Count() >= 2)
            {
                await _userDialog.AlertAsync(YouCanSelectOnlyOneSaladInThisPortition);
            }
            else if (model.Portion.GarnishList.Any(x => x.IsSelected && x.Id != model.Dish.Id))
            {
                await _userDialog.AlertAsync(YouCanSelectOnlyOneGarnishInThisPortition);
            }
            else
            {
                model.Dish.IsSelected = !model.Dish.IsSelected;

                model.Portion.PortionInfo.GarnishName = model.Portion.IsDuplicate ? DuplicatePrefix + model.Dish.Name : model.Dish.Name;
                model.Portion.PortionInfo.IsGarnishVisible = model.Dish.IsSelected;
                SetPriceForPortition(model);
                CalculateTotolPrice();
            }
        }

        private async void OnChangeCheckboxMeatCommandAsync(PortionWithCheckDish model)
        {
            if (model.Portion.MeatList.Any(x => x.IsSelected && x.Id != model.Dish.Id))
            {
                await _userDialog.AlertAsync(YouCanSelectOnlyOneMeatInThisPortition);
            }
            else
            {
                model.Dish.IsSelected = !model.Dish.IsSelected;

                model.Portion.PortionInfo.MeatName = model.Portion.IsDuplicate ? DuplicatePrefix + model.Dish.Name : model.Dish.Name;
                model.Portion.PortionInfo.IsMeatVisible = model.Dish.IsSelected;
                SetPriceForPortition(model);
                CalculateTotolPrice();
            }
        }

        private void SetPriceForPortition(PortionWithCheckDish model)
        {
            var saladSelectedCount = model.Portion.SaladList.Where(x => x.IsSelected).Count();
            if (model.Dish.IsSelected && (model.Dish.IsHightPrice || saladSelectedCount == 2))
            {
                model.Portion.PriceStr = ((decimal)DishOrderPriceType.HightPrice).ToString() + PricePostFix;
                model.Portion.Price = (decimal)DishOrderPriceType.HightPrice;
            }
            else
            {
                model.Portion.PriceStr = ((decimal)DishOrderPriceType.UsualPrice).ToString() + PricePostFix;
                model.Portion.Price = (decimal)DishOrderPriceType.UsualPrice;
            }
        }

        private async void OnChangeCheckboxSaladCommandAsync(PortionWithCheckDish model)
        {
            var countSaladSelected = model.Portion.SaladList.Where(x => x.IsSelected && x.Id != model.Dish.Id).Count();
            if ((model.Portion.GarnishList.Any(x => x.IsSelected) && model.Portion.SaladList.Any(x => x.IsSelected && x.Id != model.Dish.Id))
                || (!model.Portion.GarnishList.Any(x => x.IsSelected) && countSaladSelected >= 2))
            {
                await _userDialog.AlertAsync(YouCanSelectOnlyOneSaladInThisPortition);
            }
            else
            {
                model.Dish.IsSelected = !model.Dish.IsSelected;

                var selectedSalad = model.Portion.SaladList.Where(x => x.IsSelected).ToList();
                if (selectedSalad.Count() == 1)
                {
                    model.Portion.PortionInfo.FirstSaladName = model.Portion.IsDuplicate ? DuplicatePrefix + selectedSalad.First().Name : selectedSalad.First().Name;
                    model.Portion.PortionInfo.IsFirstSaladVisible = model.Dish.IsSelected;
                    model.Portion.PortionInfo.IsSecondSaladVisible = !model.Dish.IsSelected;
                }
                else if (selectedSalad.Count() == 2)
                {
                    model.Portion.PortionInfo.FirstSaladName = model.Portion.IsDuplicate ? DuplicatePrefix + selectedSalad.First().Name : selectedSalad.First().Name;
                    model.Portion.PortionInfo.IsFirstSaladVisible = model.Dish.IsSelected;

                    model.Portion.PortionInfo.SecondSaladName = model.Portion.IsDuplicate ? DuplicatePrefix + selectedSalad.Last().Name : selectedSalad.Last().Name;
                    model.Portion.PortionInfo.IsSecondSaladVisible = model.Dish.IsSelected;
                }
                else
                {
                    model.Portion.PortionInfo.IsFirstSaladVisible = model.Dish.IsSelected;
                    model.Portion.PortionInfo.IsSecondSaladVisible = model.Dish.IsSelected;
                }
                SetPriceForPortition(model);
                CalculateTotolPrice();
            }
        }

        private async void OnChangeCheckboxBreadCommandAsync(PortionWithCheckDish model)
        {
            if (model.Portion.BreadList.Any(x => x.IsSelected && x.Id != model.Dish.Id))
            {
                await _userDialog.AlertAsync(YouCanSelectOnlyOneBreadInThisPortition);
            }
            else
            {
                model.Dish.IsSelected = !model.Dish.IsSelected;

                model.Portion.PortionInfo.BreadName = model.Dish.Name;
                model.Portion.PortionInfo.IsBreadVisible = model.Dish.IsSelected;
                SetPriceForPortition(model);
                CalculateTotolPrice();
            }
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            
        }

        private void CalculateTotolPrice()
        {
            _totalPrise = 0;
            foreach(var portitions in PortionModels)
            {
                _totalPrise += portitions.IsDuplicate ? portitions.Price * 2 : portitions.Price;
            }
            TotalPriceStr = _totalPrise.ToString() + PricePostFix;
        }

        public async void OnNavigatedTo(NavigationParameters parameters)
        {
            PortionModels = new ObservableCollection<PortionModel>();

            var result = await _dishMenuService.GetAsync();
            if(result.IsSuccess)
            {
                if (result.Result != null)
                {
                    Date = result.Result.Date;

                    _dishes = result.Result.Dishes;

                    await CreatePortitionAsync();
                }
                else
                {
                    Date = DateTime.Now;
                }
            }
            else
            {
                await _userDialog.AlertAsync(Constants.ErrorWithSendDataToServer);
            }
        }

        public async Task CreatePortitionAsync()
        {
            PortionModel portitionModel = new PortionModel();

            portitionModel.GarnishList = new ObservableCollection<DishModel>(_dishes.Where(x => x.CategoryType == DishCategory.Garnish).Select(x=> CreateNewModel(x)));
            portitionModel.MeatList = new ObservableCollection<DishModel>(_dishes.Where(x => x.CategoryType == DishCategory.Meat).Select(x => CreateNewModel(x)));
            portitionModel.SaladList = new ObservableCollection<DishModel>(_dishes.Where(x => x.CategoryType == DishCategory.Salad).Select(x => CreateNewModel(x)));
            portitionModel.BreadList = new ObservableCollection<DishModel>(_dishes.Where(x => x.CategoryType == DishCategory.Bread).Select(x => CreateNewModel(x)));

            portitionModel.ChangeVisiblePortitionCommand = new Command<PortionModel>(OnChangeVisiblePortitionCommandAsync);
            portitionModel.ChangeCheckboxIsDuplicateCommand = new Command<PortionModel>(OnChangeCheckboxIsDuplicateCommandAsync);
            portitionModel.DeletePortionCommand = new Command<PortionModel>(OnDeletePortionCommandAsync);

            portitionModel.PortionInfo = new OrderDishModel();
            portitionModel.PortionInfo.Portition = portitionModel;

            portitionModel.PriceStr = ((decimal)DishOrderPriceType.UsualPrice).ToString() + PricePostFix;
            portitionModel.Price = (decimal)DishOrderPriceType.UsualPrice;

            foreach (var dish in portitionModel.GarnishList)
            {
                dish.ChangeCheckboxCommand = new Command<PortionWithCheckDish>(OnChangeCheckboxGarnishCommandAsync);
            }

            foreach (var dish in portitionModel.MeatList)
            {
                dish.ChangeCheckboxCommand = new Command<PortionWithCheckDish>(OnChangeCheckboxMeatCommandAsync);
            }

            foreach (var dish in portitionModel.SaladList)
            {
                dish.ChangeCheckboxCommand = new Command<PortionWithCheckDish>(OnChangeCheckboxSaladCommandAsync);
            }

            foreach (var dish in portitionModel.BreadList)
            {
                dish.ChangeCheckboxCommand = new Command<PortionWithCheckDish>(OnChangeCheckboxBreadCommandAsync);
            }

            PortionModels.Add(portitionModel);
            CalculateTotolPrice();
        }

        private DishModel CreateNewModel(DishModel model)
        => new DishModel()
        {
            CategoryType = model.CategoryType,
            IsHightPrice = model.IsHightPrice,
            Id = model.Id,
            Name = model.Name,
            IsSelected = false
        };

        private async void OnDeletePortionCommandAsync(PortionModel model)
        {
            PortionModels.Remove(model);
            CalculateTotolPrice();
        }

        private async Task OnAddPortionAsync()
        {
            bool isCheckedPortion = CheckValidationLastPortion();
            if(isCheckedPortion)
            {
                await CreatePortitionAsync();
            }
            else
            {
                await _userDialog.AlertAsync(YouCanNotCreateNewPortion);
            }
        }

        private bool CheckValidationLastPortion()
        {
            bool isChecked = false;
            var lastPortion = PortionModels.LastOrDefault();
            if(lastPortion != null)
            {
                if((lastPortion.PortionInfo.IsGarnishVisible
                   && lastPortion.PortionInfo.IsMeatVisible
                    && lastPortion.PortionInfo.IsGarnishVisible)
                   || 
                   (lastPortion.PortionInfo.IsMeatVisible
                    && lastPortion.PortionInfo.IsFirstSaladVisible
                    && lastPortion.PortionInfo.IsSecondSaladVisible
                    && lastPortion.PortionInfo.IsBreadVisible))
                {
                    isChecked = true;
                    lastPortion.IsVisible = false;
                }
            }
            else
            {
                isChecked = true;
            }
            return isChecked;
        }
    }
}
