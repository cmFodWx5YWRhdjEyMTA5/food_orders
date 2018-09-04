using System;
using HWOrderFood.ModelLayers.Dish;

namespace HWOrderFood.ModelLayers.Portition.Models
{
    public class PortionWithCheckDish
    {
        public PortionModel Portion { get; set; }

        public DishModel Dish { get; set; }
    }
}
