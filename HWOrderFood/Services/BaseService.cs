using System;
using System.Threading.Tasks;
using HWOrderFood.ModelLayers;

namespace HWOrderFood.Services
{
    public class BaseService
    {
        protected string NameController { get; set; }

        protected AOResult<T> BaseInvoke<T>(Action<AOResult<T>> action)
        {
            AOResult<T> aoResult = new AOResult<T>();
            try
            {
                action(aoResult);
            }
            catch(Exception ex)
            {
                aoResult.SetError(null, ex.Message, ex);
            }
            return aoResult;
        }

        protected async Task<AOResult<T>> BaseInvokeAsync<T>(Func<AOResult<T>, Task> action)
        {
            AOResult<T> aoResult = new AOResult<T>();
            try
            {
                await action(aoResult);
            }
            catch (Exception ex)
            {
                aoResult.SetError(null, ex.Message, ex);
            }
            return aoResult;
        }
    }
}
