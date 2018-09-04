using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HWOrderFood.ModelLayers;
using HWOrderFood.ModelLayers.Dish;

namespace HWOrderFood.Services.Dish
{
    public interface IDishService
    {
        Task<AOResult> CreateAsync(DishModel dish);
        Task<AOResult<IEnumerable<DishModel>>> GetAsync();
        Task<AOResult> DeleteAsync(int id);
    }
}
