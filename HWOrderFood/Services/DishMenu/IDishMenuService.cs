using System;
using System.Threading.Tasks;
using HWOrderFood.ModelLayers;
using HWOrderFood.ModelLayers.DishMenu.Models;
using HWOrderFood.ModelLayers.DishMenu.Requests;

namespace HWOrderFood.Services.DishMenu
{
    public interface IDishMenuService
    {
        Task<AOResult> CreateAsync(DishMenuRequest request);
        Task<AOResult<DishMenuModel>> GetAsync();
    }
}
