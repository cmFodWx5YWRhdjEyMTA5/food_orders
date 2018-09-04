using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using HWOrderFood.ModelLayers.Dish;
using HWOrderFood.ModelLayers.OrderDish.Models;
using Prism.Mvvm;

namespace HWOrderFood.ModelLayers.Portition.Models
{
    public class PortionModel : BindableBase
    {
        private ObservableCollection<DishModel> _garnishList;
        public ObservableCollection<DishModel> GarnishList
        {
            get { return _garnishList; }
            set { SetProperty(ref _garnishList, value); }
        }

        private ObservableCollection<DishModel> _meatList;
        public ObservableCollection<DishModel> MeatList
        {
            get { return _meatList; }
            set { SetProperty(ref _meatList, value); }
        }

        private ObservableCollection<DishModel> _saladList;
        public ObservableCollection<DishModel> SaladList
        {
            get { return _saladList; }
            set { SetProperty(ref _saladList, value); }
        }

        private ObservableCollection<DishModel> _breadList;
        public ObservableCollection<DishModel> BreadList
        {
            get { return _breadList; }
            set { SetProperty(ref _breadList, value); }
        }

        private bool _isVisible;
        public bool IsVisible
        {
            get { return _isVisible; }
            set { SetProperty(ref _isVisible, value); }
        }

        private bool _isToggled;
        public bool IsToggled
        {
            get { return _isToggled; }
            set { SetProperty(ref _isToggled, value); }
        }

        private string _comment;
        public string Comment
        {
            get { return _comment; }
            set { SetProperty(ref _comment, value); }
        }

        private bool _isDuplicate;
        public bool IsDuplicate
        {
            get { return _isDuplicate; }
            set { SetProperty(ref _isDuplicate, value); }
        }

        private string _priceStr;
        public string PriceStr
        {
            get { return _priceStr; }
            set { SetProperty(ref _priceStr, value); }
        }

        public decimal Price { get; set; }

        private OrderDishModel _portionInfo;
        public OrderDishModel PortionInfo
        {
            get { return _portionInfo; }
            set { SetProperty(ref _portionInfo, value); }
        }

        private ICommand _changeCheckboxIsDuplicateCommand;
        public ICommand ChangeCheckboxIsDuplicateCommand
        {
            get { return _changeCheckboxIsDuplicateCommand; }
            set { SetProperty(ref _changeCheckboxIsDuplicateCommand, value); }
        }

        private ICommand _changeVisiblePortitionCommand;
        public ICommand ChangeVisiblePortitionCommand
        {
            get { return _changeVisiblePortitionCommand; }
            set { SetProperty(ref _changeVisiblePortitionCommand, value); }
        }

        private ICommand _deletePortionCommand;
        public ICommand DeletePortionCommand
        {
            get { return _deletePortionCommand; }
            set { SetProperty(ref _deletePortionCommand, value); }
        }
        //private ICommand _changeTogglePortitionCommand;
        //public ICommand ChangeTogglePortitionCommand
        //{
        //    get { return _changeTogglePortitionCommand; }
        //    set { SetProperty(ref _changeTogglePortitionCommand, value); }
        //}
    }
}
