using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using Android.App;
using Android.Widget;
using HWOrderFood.Enums;
using HWOrderFood.Helpers;
using HWOrderFood.ModelLayers.Dish;
using HWOrderFood.Services.Dish;
using HWOrderFood.Views;
using Prism.Navigation;

namespace HWOrderFood.ViewModels
{
    public class AddDishViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly IDishService _dishService;
        private readonly IUserDialogs _userDialog;
        private const string DishNameMustNotBeEmpty = "Название блюда не должно быть пустым";
        private const string CategoryDishMustBeSelected = "Категория блюда должна быть выбрана";
        private const string ServerError = "Ошибка сервера";

        public AddDishViewModel(INavigationService navigationService,
                                IUserDialogs userDialogs,
                                IDishService dishService) : base(navigationService)
        {
            _navigationService = navigationService;
            _userDialog = userDialogs;
            _dishService = dishService;
        }

        private string _dishName;
        public string DishName
        {
            get { return _dishName; }
            set { SetProperty(ref _dishName, value); }
        }

        private string _categoryType;
        public string CategoryType
        {
            get { return _categoryType; }
            set { SetProperty(ref _categoryType, value); }
        }


        private ICommand _saveDishCommand;
        public ICommand SaveDishCommand
        {
            get
            {
                return _saveDishCommand ?? (_saveDishCommand = SingleExecutionCommand.FromFunc(OnSaveDishCommandAsync));
            }
        }

        public async Task OnSaveDishCommandAsync()
        {
            if (string.IsNullOrWhiteSpace(_dishName))
            {
                await _userDialog.AlertAsync(DishNameMustNotBeEmpty);
                return;
            }
            if (string.IsNullOrWhiteSpace(_categoryType))
            {
                await _userDialog.AlertAsync(CategoryDishMustBeSelected);
                return;
            }

            DishModel dish = new DishModel()
            {
                Name = _dishName,
                CategoryType = GetCategoryFromPicker(_categoryType)
            };
            var result = await _dishService.CreateAsync(dish);
            if (result.IsSuccess)
                await OnDishCommandAsync();
            else
                await _userDialog.AlertAsync(ServerError);
        }


        #region -- Private helpers --

        private DishCategory GetCategoryFromPicker(string category)
        {
            DishCategory categoryType = 0;
            switch(category)
            {
                case "Гарнир":
                    categoryType = DishCategory.Garnish;
                    break;
                case "Мясное":
                    categoryType = DishCategory.Meat;
                    break;
                case "Салат":
                    categoryType = DishCategory.Salad;
                    break;
                case "Хлеб":
                    categoryType = DishCategory.Bread;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return categoryType;
        }

        #endregion

    }
}
