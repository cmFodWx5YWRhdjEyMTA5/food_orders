using System;
using System.Threading.Tasks;

namespace HWOrderFood.Services.DiskCache
{
    public interface IDiskCacheService
    {
        Task<T> GetAsync<T>(string key, Func<Task<T>> valFunc, TimeSpan experation);
    }
}
