using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;

namespace TrollChat.Web.Helpers
{
    public static class TempDataExtensionsHelper
    {
        public static void Put<T>(this ITempDataDictionary tempData, string key, T value) where T : class
        {
            tempData[key] = JsonConvert.SerializeObject(value);
        }

        public static T Get<T>(this ITempDataDictionary tempData, string key) where T : class
        {
            tempData.TryGetValue(key, out var obj);

            return obj == null ? null : JsonConvert.DeserializeObject<T>((string)obj);
        }
    }
}