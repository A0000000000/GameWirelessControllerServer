using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConnectionServiceServer.Raw;
using ConnectionServiceServer.Utils;

namespace ConnectionServiceServer.Template
{
    public class ConnectionServiceClientDecorator<T> :IDisposable where T:class 
    {

        public ConnectionServiceClient ConnectionServiceClient
        {
            get;
            private set;
        }

        public ConnectionServiceClientDecorator(ConnectionServiceClient connectionServiceClient)
        {
            ConnectionServiceClient = connectionServiceClient;
        }

        
        public void Init(Action<T> onDataReadyRead, Action<Exception> onReadException)
        {
            ConnectionServiceClient.Init(data => 
            {
                string json = Encoding.UTF8.GetString(data);
                T obj = JsonUtils<T>.FromJson(json);
                if (onDataReadyRead != null)
                {
                    onDataReadyRead(obj);
                }
            }, onReadException);
        }


        public void Write(T obj)
        {
            if (obj != null)
            {
                string data = JsonUtils<T>.ToJson(obj);
                ConnectionServiceClient.Write(Encoding.UTF8.GetBytes(data));
            }
        }

        public void Dispose()
        {
            ConnectionServiceClient.Dispose();
        }

    }
}
