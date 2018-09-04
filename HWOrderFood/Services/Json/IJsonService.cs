using System;
namespace HWOrderFood.Services.Json
{
    public interface IJsonService
    {
        string SerializeObject(object value);

        T DeserializeObject<T>(string value);
    }
}
