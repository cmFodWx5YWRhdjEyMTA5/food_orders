using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using HWOrderFood.Enums;
using HWOrderFood.Helpers;
using HWOrderFood.ModelLayers;

namespace HWOrderFood.Services.Rest
{
    public class BaseRestService : IBaseRestService
    {
        public BaseRestService(IRestService restService)
        {
            RestService = restService;
        }

        protected IRestService RestService { get; private set; }

        #region -- IBaseServiceWithAuthorization implementation --

        public Task<AOResult<T>> DeleteAsync<T>(string resource, object requestBody, string callMethodName = null)
        {
            return RequestAsync<T>(RequstType.Delete, resource, requestBody, TimeSpan.Zero, StorageType.Memory, callMethodName);
        }

        public Task<AOResult<T>> GetAsync<T>(string resource, string callMethodName = null)
        {
            return RequestAsync<T>(RequstType.Get, resource, null, TimeSpan.Zero, StorageType.Memory, callMethodName);
        }

        public Task<AOResult<T>> GetAsyncWithCache<T>(string resource, TimeSpan experation, StorageType storageType, [CallerMemberName] string callMethodName = null)
        {
            return RequestAsync<T>(RequstType.GetWithCache, resource, null, experation, storageType, callMethodName);
        }

        public Task<AOResult<T>> PostAsync<T>(string resource, object requestBody, string callMethodName = null)
        {
            return RequestAsync<T>(RequstType.Post, resource, requestBody, TimeSpan.Zero, StorageType.Memory, callMethodName);
        }

        public Task<AOResult<T>> PutAsync<T>(string resource, object requestBody, string callMethodName = null)
        {
            return RequestAsync<T>(RequstType.Put, resource, requestBody, TimeSpan.Zero, StorageType.Memory, callMethodName);
        }

        #endregion

        #region -- Private helpers --

        private async Task<AOResult<T>> RequestAsync<T>(RequstType requestType, string resource, object requestBody, TimeSpan experation, StorageType storageType, string callMethodName)
        {
            var aoResult = new AOResult<T>();

            T result = default(T);

            try
            {
                switch (requestType)
                {
                    case RequstType.Post:
                        result = await RestService.PostAsync<T>(resource, requestBody);
                        break;
                    case RequstType.Get:
                        result = await RestService.GetAsync<T>(resource);
                        break;
                    case RequstType.GetWithCache:
                        result = await RestService.GetAsyncWithCashe<T>(resource, experation, storageType);
                        break;
                    case RequstType.Put:
                        result = await RestService.PutAsync<T>(resource, requestBody);
                        break;
                    case RequstType.Delete:
                        result = await RestService.DeleteAsync<T>(resource, requestBody);
                        break;
                    default:
                        throw new InvalidOperationException("Wrong request type");
                }
                aoResult.SetSuccess(result);
            }
            catch (WebException ex)
            {
                ErrorsTracker.TrackError(ex, ex.Message);
                aoResult.SetError(callMethodName + "_exception", ex.Message, ex);
                Debug.WriteLine("## Error rest service || " + callMethodName + " **** " + resource);
            }
            catch (HttpRequestException ex)
            {
                ErrorsTracker.TrackError(ex, ex.Message);
                aoResult.SetError(callMethodName + "_exception", ex.Message, ex);
                Debug.WriteLine("## Error rest service || " + callMethodName + " **** " + resource);
            }
            catch (Exception ex)
            {
                ErrorsTracker.TrackError(ex, ex.Message);
                throw;
            }
            return aoResult;
        }

        private enum RequstType
        {
            Post,
            Get,
            Put,
            Delete,
            GetWithCache
        }
        #endregion
    }
}
