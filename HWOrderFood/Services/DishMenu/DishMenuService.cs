using System;
using System.Threading.Tasks;
using HWOrderFood.ModelLayers;
using HWOrderFood.ModelLayers.DishMenu.Models;
using HWOrderFood.ModelLayers.DishMenu.Requests;
using HWOrderFood.ModelLayers.Response;
using HWOrderFood.Services.Rest;

namespace HWOrderFood.Services.DishMenu
{
    public class DishMenuService : BaseService, IDishMenuService
    {
        private readonly IBaseRestService _baseRestService;

        public DishMenuService(IBaseRestService baseRestService)
        {
            _baseRestService = baseRestService;
            NameController = "DishMenu";
        }

        public async Task<AOResult> CreateAsync(DishMenuRequest request)
        => await BaseInvokeAsync<Task<AOResult>>(async (aoResult) =>
        {
            var response = await _baseRestService.PostAsync<ResponseModel>(NameController, request);
            if(response !=null && response.IsSuccess)
            {
                if (response.Result.IsSuccess)
                    aoResult.SetSuccess();
                else
                    aoResult.SetFailure();
            }
            else
                aoResult.SetFailure();
        });

        public async Task<AOResult<DishMenuModel>> GetAsync()
        => await BaseInvokeAsync<DishMenuModel>(async (aoResult) =>
        {
            var response = await _baseRestService.GetAsync<ResponseModel<DishMenuModel>>(NameController);
            if(response != null && response.IsSuccess)
            {
                var dishMenuResult = response.Result;
                if(dishMenuResult != null && dishMenuResult.IsSuccess)
                {
                    aoResult.SetSuccess(dishMenuResult.Result);
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
        });
    }
}
