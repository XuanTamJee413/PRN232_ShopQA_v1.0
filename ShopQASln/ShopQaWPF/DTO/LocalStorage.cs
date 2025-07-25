using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ShopQaWPF.DTO
{
    public static class LocalStorage
    {
        private static readonly Dictionary<string, string> data = new();

        public static void Set(string key, object value)
        {
            data[key] = JsonSerializer.Serialize(value);
        }

        public static T Get<T>(string key)
        {
            return data.ContainsKey(key) ? JsonSerializer.Deserialize<T>(data[key]) : default;
        }

        public static string Get(string key)
        {
            return data.ContainsKey(key) ? JsonSerializer.Deserialize<string>(data[key]) : null;
        }
    }

}
