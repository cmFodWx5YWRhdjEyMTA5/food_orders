using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using HWOrderFood.Enums;
using HWOrderFood.ModelLayers;

namespace HWOrderFood.Services.Rest
{
    public interface IBaseRestService
    {
        Task<AOResult<T>> GetAsyncWithCache<T>(string resource, TimeSpan experation, StorageType storageType, [CallerMemberName] string callMethodName = null);

        Task<AOResult<T>> GetAsync<T>(string resource, string callMethodName = null);

        Task<AOResult<T>> PutAsync<T>(string resource, object requestBody, string callMethodName = null);

        Task<AOResult<T>> DeleteAsync<T>(string resource, object requestBody, string callMethodName = null);

        Task<AOResult<T>> PostAsync<T>(string resource, object requestBody, string callMethodName = null);
    }
}
