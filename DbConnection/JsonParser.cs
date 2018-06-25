using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DbConnection
{

    public static class JsonParser<T>
    {
        public static T getClass(string source)
        {
            var obj = JsonConvert.DeserializeObject<T>(source);
            return obj;
        }

        public static List<string> InvalidJsonElements;

        public static IList<T> DeserializeToList(string jsonString)
        {
            InvalidJsonElements = null;

            var array = JArray.Parse(jsonString);

            IList<T> objectsList = new List<T>();

            foreach (var item in array)
            {
                try
                {
                    // CorrectElements
                    objectsList.Add(item.ToObject<T>());
                }
                catch (Exception)
                {
                    InvalidJsonElements = InvalidJsonElements ?? new List<string>();
                    InvalidJsonElements.Add(item.ToString());
                }
            }
            return objectsList;
        }
    }
}
