using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GameWirelessControllerServer
{
    public class TransferObject
    {
        public static TransferObject FromJson(string json)
        {
            return JsonSerializer.Deserialize<TransferObject>(json);
        }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }

        public TransferObject(Dictionary<string, object> data, int type, string message)
        {
            this.Data = data;
            this.Type = type;
            this.Message = message;
        }


        public Dictionary<string, object> Data
        {
            get;
            set;
        }

        public int Type
        {
            get;
            set;
        }

        public string Message
        {
            get;
            set;
        }


        public override string ToString()
        {
            if (Type != 0 || Data == null)
            {
                return Message;
            }
            StringBuilder sb = new StringBuilder();
            foreach(KeyValuePair<string, object> kv in Data)
            {
                sb.Append($"{kv.Key} : {kv.Value}\n");
            }
            return sb.ToString();
        }

    }
}
