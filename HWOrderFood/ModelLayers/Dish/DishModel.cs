using System;
using System.Threading.Tasks;
using System.Windows.Input;
using HWOrderFood.Enums;
using HWOrderFood.Helpers;
using Newtonsoft.Json;
using Prism.Mvvm;

namespace HWOrderFood.ModelLayers.Dish
{
    public class DishModel : BindableBase
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("dishCategory")]
        public DishCategory CategoryType { get; set; }

        [JsonIgnore]
        //[JsonProperty("isHightPrice")]
        public bool IsHightPrice { get; set; }

        private bool _isSelected;
        [JsonIgnore]
        public bool IsSelected {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value); }
        }

        private ICommand _deleteMeCommand;
        [JsonIgnore]
        public ICommand DeleteMeCommand
        {
            get { return _deleteMeCommand; }
            set { SetProperty(ref _deleteMeCommand, value); }
        }

        private ICommand _changeCheckboxCommand;
        public ICommand ChangeCheckboxCommand
        {
            get { return _changeCheckboxCommand; }
            set { SetProperty(ref _changeCheckboxCommand, value); }
        }
    }
}
