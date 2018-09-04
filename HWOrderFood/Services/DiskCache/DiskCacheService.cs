using System;
using System.Threading.Tasks;
using Akavache;
using HWOrderFood.Helpers;
using System.Reactive.Linq;

namespace HWOrderFood.Services.DiskCache
{
    public class DiskCacheService : IDiskCacheService
    {
        #region -- IDiskCacheService implementation --

        public async Task<T> GetAsync<T>(string key, Func<Task<T>> valFunc, TimeSpan experation)
        {
            var isExist = await CanGetObject<T>(key);

            if (isExist)
            {
                return await Get<T>(key);
            }
            else
            {
                try
                {
                    var res = await valFunc();
                    await Set(key, res, experation);

                    return res;
                }
                catch(Exception ex)
                {
                    ErrorsTracker.TrackError(ex, ex.Message);
                    throw;
                }
            }
        }

        #endregion

        #region -- Private helpers --

        private async Task<bool> CanGetObject<T>(string key)
        {
            try
            {
                await BlobCache.LocalMachine.GetObject<T>(key);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task<T> Get<T>(string key)
        {
            return await BlobCache.LocalMachine.GetObject<T>(key);
        }

        private async Task Set<T>(string key, T val, TimeSpan experation)
        {
            await BlobCache.LocalMachine.InsertObject(key, val, experation);
        }

        #endregion

    }
}
