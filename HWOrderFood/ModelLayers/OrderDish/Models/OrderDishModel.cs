using System;
using System.Windows.Input;
using HWOrderFood.ModelLayers.Dish;
using HWOrderFood.ModelLayers.Portition.Models;
using Prism.Mvvm;

namespace HWOrderFood.ModelLayers.OrderDish.Models
{
    public class OrderDishModel : BindableBase
    {
        public int GarnishId { get; set; }

        private string _garnishName;
        public string GarnishName
        {
            get { return _garnishName; }
            set { SetProperty(ref _garnishName, value); }
        }

        private bool _isGarnishVisible;
        public bool IsGarnishVisible
        {
            get { return _isGarnishVisible; }
            set { SetProperty(ref _isGarnishVisible, value); }
        }

        public int MeatId { get; set; }

        private string _meatName;
        public string MeatName
        {
            get { return _meatName; }
            set { SetProperty(ref _meatName, value); }
        }

        private bool _isMeatVisible;
        public bool IsMeatVisible
        {
            get { return _isMeatVisible; }
            set { SetProperty(ref _isMeatVisible, value); }
        }

        public int FirstSaladId { get; set; }

        private string _firstSaladName;
        public string FirstSaladName
        {
            get { return _firstSaladName; }
            set { SetProperty(ref _firstSaladName, value); }
        }

        private bool _isFirstSaladVisible;
        public bool IsFirstSaladVisible
        {
            get { return _isFirstSaladVisible; }
            set { SetProperty(ref _isFirstSaladVisible, value); }
        }

        public int SecondSaladId { get; set; }

        private string _secondSaladName;
        public string SecondSaladName
        { 
            get { return _secondSaladName; }
            set { SetProperty(ref _secondSaladName, value); } 
        }

        private bool _isSecondSaladVisible;
        public bool IsSecondSaladVisible
        {
            get { return _isSecondSaladVisible; }
            set { SetProperty(ref _isSecondSaladVisible, value); }
        }

        public int BreadId { get; set; }

        private bool _isBreadVisible;
        public bool IsBreadVisible
        {
            get { return _isBreadVisible; }
            set { SetProperty(ref _isBreadVisible, value); }
        }

        private string _breadName;
        public string BreadName
        {
            get { return _breadName; }
            set { SetProperty(ref _breadName, value); }
        }

        private PortionModel _portition;
        public PortionModel Portition
        {
            get { return _portition; }
            set { SetProperty(ref _portition, value); }
        }
    }
}
