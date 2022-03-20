using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace ConnectionServiceServer.Utils
{
    public static class JsonUtils<T> where T: class
    {
        public static T FromJson(string json)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(json);
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public static string ToJson(T obj)
        {
            try
            {
                return JsonSerializer.Serialize(obj);
            }
            catch (Exception e)
            {
                return String.Empty;
            }
        }

    }
}
