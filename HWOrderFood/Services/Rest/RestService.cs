using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HWOrderFood.Enums;
using HWOrderFood.Helpers;
using HWOrderFood.Services.DiskCache;
using HWOrderFood.Services.Json;

namespace HWOrderFood.Services.Rest
{
    public class RestService : IRestService
    {
        private readonly MemoryCash<string, object> _memoryCash = new MemoryCash<string, object>(TimeSpan.FromMinutes(20));
        private readonly IJsonService _jsonService;
        private readonly IDiskCacheService _diskCacheService;

        public RestService(IJsonService jsonService, IDiskCacheService diskCacheService)
        {
            BaseUrl = Constants.HWOrderFood_BASE_URL;
            _jsonService = jsonService;
            _diskCacheService = diskCacheService;
        }

        public string BaseUrl { get; private set; }

        #region -- IRestService implementation --

        public async Task<T> DeleteAsync<T>(string resource, Dictionary<string, string> additioalHeaders = null)
        {
            using (var client = CreateHttpClient(DefaultHeaders(additioalHeaders)))
            {
                using (var response = await client.DeleteAsync(GetRequestUrl(BaseUrl, resource)))
                {
                    ThrowIfNotSuccess(response);

                    var data = await response.Content.ReadAsStringAsync();

                    return _jsonService.DeserializeObject<T>(data);
                }
            }
        }

        public async Task<T> DeleteAsync<T>(string resource, object requestBody, Dictionary<string, string> additioalHeaders = null)
        {
            using (var client = CreateHttpClient(DefaultHeaders(additioalHeaders)))
            {
                var jsonString = _jsonService.SerializeObject(requestBody);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                using (var request = new HttpRequestMessage
                {
                    Content = content,
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(GetRequestUrl(BaseUrl, resource))
                })
                {
                    using (var response = await client.SendAsync(request))
                    {
                        ThrowIfNotSuccess(response);

                        var data = await response.Content.ReadAsStringAsync();

                        return _jsonService.DeserializeObject<T>(data);
                    }
                }
            }
        }

        public async Task<T> GetAsync<T>(string resource, Dictionary<string, string> additioalHeaders = null)
        {
            using (var client = CreateHttpClient(DefaultHeaders(additioalHeaders)))
            {
                using (var response = await client.GetAsync(GetRequestUrl(BaseUrl, resource)))
                {
                    ThrowIfNotSuccess(response);

                    var data = await response.Content.ReadAsStringAsync();

                    var t = _jsonService.DeserializeObject<T>(data);

                    return t;
                }
            }
        }

        public async Task<T> GetAsyncWithCashe<T>(string resource, TimeSpan experation, StorageType storageType, Dictionary<string, string> additioalHeaders = null)
        {
            if (storageType == StorageType.Memory)
            {
                return (T)await _memoryCash.GetAsync(resource, async () =>
                {
                    var res = await GetAsync<T>(resource, additioalHeaders);
                    return (object)res;
                });
            }
            else if (storageType == StorageType.Disk)
            {
                return await _diskCacheService.GetAsync<T>(resource, async () =>
                {
                    return await GetAsync<T>(resource, additioalHeaders);
                }, experation);
            }
            return default(T);
        }

        public Task<T> PostAsync<T>(string resource, object requestBody, Dictionary<string, string> additioalHeaders = null)
        {
            var jsonString = _jsonService.SerializeObject(requestBody);

            HttpContent content = requestBody as HttpContent;
            if (requestBody is IEnumerable<KeyValuePair<string, string>>)
                content = new FormUrlEncodedContent(requestBody as IEnumerable<KeyValuePair<string, string>>);
            if (content == null)
                content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            return PostAsync<T>(resource, content, CancellationToken.None, additioalHeaders); ;
        }

        public Task<T> PutAsync<T>(string resource, object requestBody, Dictionary<string, string> additioalHeaders = null)
        {
            var jsonString = _jsonService.SerializeObject(requestBody);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            return PutAsync<T>(resource, requestBody, additioalHeaders);
        }

        #endregion

        #region -- Private helpers --

        private async Task<T> PostAsync<T>(string resource, HttpContent content, CancellationToken cancellationToken, Dictionary<string, string> additioalHeaders = null)
        {
            try
            {
                using (var client = CreateHttpClient(DefaultHeaders(additioalHeaders)))
                {
                    using (var response = await client.PostAsync(GetRequestUrl(BaseUrl, resource), content, cancellationToken))
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        ThrowIfNotSuccess(response);

                        var data = await response.Content.ReadAsStringAsync();
                        return _jsonService.DeserializeObject<T>(data);
                    }
                }
            }
            catch(Exception ex)
            {
                int i = 0;
                throw;
            }
        }

        private async Task<T> PutAsync<T>(string resource, HttpContent content, CancellationToken cancellationToken, Dictionary<string, string> additioalHeaders = null)
        {
            using (var client = CreateHttpClient(DefaultHeaders(additioalHeaders)))
            {
                using (var response = await client.PutAsync(GetRequestUrl(BaseUrl, resource), content, cancellationToken))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    ThrowIfNotSuccess(response);

                    var data = await response.Content.ReadAsStringAsync();
                    return _jsonService.DeserializeObject<T>(data);
                }
            }
        }


        private static void ThrowIfNotSuccess(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                response.EnsureSuccessStatusCode();
            }
        }

        internal string BuildParametersString(Dictionary<string, string> parameters)
        {
            if (parameters == null || parameters.Count == 0)
                return string.Empty;

            var sb = new StringBuilder("?");
            bool needAddDivider = false;

            foreach (var item in parameters)
            {
                if (needAddDivider)
                    sb.Append('&');
                var encodedKey = WebUtility.UrlEncode(item.Key);
                var encodedVal = WebUtility.UrlEncode(item.Value);
                sb.Append($"{encodedKey}={encodedVal}");

                needAddDivider = true;
            }

            return sb.ToString();
        }

        internal string GetRequestUrl(string host, string resource, Dictionary<string, string> parameters = null)
        {
            var ret = string.Empty;
            string paramsStr = BuildParametersString(parameters);
            if (resource.Contains("local"))
                ret = $"{resource}{paramsStr}";
            else
                ret = $"{host}{resource}{paramsStr}";
            return ret;
        }

        private Dictionary<string, string> DefaultHeaders(Dictionary<string, string> additioalHeaders = null)
        {
            var defheaders = new Dictionary<string, string>();
            defheaders["User-Agent"] = "Mobile";
            defheaders["Accept"] = "application/json";

            if (additioalHeaders != null)
                foreach (var kv in additioalHeaders)
                {
                    defheaders[kv.Key] = kv.Value;
                }
            return defheaders;
        }


        private HttpClient CreateHttpClient(Dictionary<string, string> headerParams = null)
        {
            var httpClient = new HttpClient();

            if (headerParams != null)
            {
                foreach (var headerParam in headerParams)
                {
                    httpClient.DefaultRequestHeaders.Add(headerParam.Key, headerParam.Value);
                }
            }

            return httpClient;
        }


        #endregion
    }
}
