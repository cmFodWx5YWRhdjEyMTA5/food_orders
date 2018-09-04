using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HWOrderFood.ModelLayers;
using HWOrderFood.ModelLayers.Dish;
using HWOrderFood.ModelLayers.Response;
using HWOrderFood.Services.Rest;

namespace HWOrderFood.Services.Dish
{
    public class DishService : BaseService, IDishService
    {
        private readonly IBaseRestService _baseService;

        public DishService(IBaseRestService baseService)
        {
            _baseService = baseService;
            NameController = "Dish";

        }

        public async Task<AOResult> CreateAsync(DishModel dish)
        {
            var result = new AOResult();
            try
            {
                var content = new
                {
                    Name = dish.Name,
                    DishCategory = dish.CategoryType 
                };
                var response = await _baseService.PostAsync<ResponseModel>(NameController, content);
                if(response != null)
                {
                    if (response.IsSuccess)
                        result.SetSuccess();
                    else
                    {
                        result.SetFailure();
                    }
                }
            }
            catch(Exception ex)
            {
                result.SetError(nameof(CreateAsync), ex.Message, ex);
            }
            return result;
        }

        public async Task<AOResult<IEnumerable<DishModel>>> GetAsync()
        => await BaseInvokeAsync<IEnumerable<DishModel>>(async (aoResult) =>
        {
            var response = await _baseService.GetAsync<ResponseModel<IEnumerable<DishModel>>>(NameController);

            if (response != null)
            {
                if (response.IsSuccess && response.Result != null)
                {
                    var dishResult = response.Result;
                    if (dishResult != null && dishResult.IsSuccess)
                    {
                        aoResult.SetSuccess(dishResult.Result);
                    }
                    else
                    {
                        aoResult.SetFailure();
                    }
                }
                else
                {
                    aoResult.SetFailure();
                }
            }
        });


        public async Task<AOResult> DeleteAsync(int id)
        => await BaseInvokeAsync<Task<AOResult>>(async (aoResult) =>
        {
            var request = string.Format(NameController + "?id={0}", id);
            var response = await _baseService.DeleteAsync<ResponseModel>(request, null);
            if(response != null && response.IsSuccess)
            {
                aoResult.SetSuccess();
            }
            else
            {
                aoResult.SetFailure();
            }
        });
           
    }
}
