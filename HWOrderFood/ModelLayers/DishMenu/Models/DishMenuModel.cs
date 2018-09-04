using System;
using System.Collections;
using System.Collections.Generic;
using HWOrderFood.ModelLayers.Dish;
using Newtonsoft.Json;

namespace HWOrderFood.ModelLayers.DishMenu.Models
{
    public class DishMenuModel
    {
        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("dishes")]
        public IEnumerable<DishModel> Dishes { get; set; }
    }
}
