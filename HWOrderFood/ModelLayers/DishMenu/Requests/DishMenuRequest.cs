using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace HWOrderFood.ModelLayers.DishMenu.Requests
{
    public class DishMenuRequest
    {
        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("dishIds")]
        public IEnumerable<int> DishIds { get; set; }
    }
}
