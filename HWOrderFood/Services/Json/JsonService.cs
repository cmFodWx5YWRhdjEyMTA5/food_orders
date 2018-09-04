using System;
using Newtonsoft.Json;

namespace HWOrderFood.Services.Json
{
    public class JsonService : IJsonService
    {
        #region -- IJsonService implementation --

        public string SerializeObject(object value)
        {
            return JsonConvert.SerializeObject(value);
        }

        public T DeserializeObject<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }

        #endregion
    }
}
