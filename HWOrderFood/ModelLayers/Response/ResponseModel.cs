using System;
using System.Collections.Generic;
using HWOrderFood.Enums;
using Newtonsoft.Json;

namespace HWOrderFood.ModelLayers.Response
{
    public class ResponseModel<T> : ResponseModel
    {
        [JsonProperty("result")]
        public T Result { get; set; }
    }

    public class ResponseModel
    {
        [JsonProperty("ok")]
        public bool IsSuccess { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("errors")]
        public IEnumerable<Error> Errors { get; set; }

        [JsonProperty("code")]
        public StatusCode StatusError { get; set; }
    }

    public class Error
    {
        public int StatusCode { get; set; }
        public string Key { get; set; }
        public string Message { get; set; }
    }
}
