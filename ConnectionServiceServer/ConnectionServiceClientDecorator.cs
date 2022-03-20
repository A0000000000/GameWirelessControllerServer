using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConnectionServiceServer.Internal;
using ConnectionServiceServer.Utils;

namespace ConnectionServiceServer
{
    public class ConnectionServiceClientDecorator<T> :IDisposable where T:class 
    {
        public string Name
        {
            get;
            private set;
        }

        public ConnectionServiceClient ConnectionServiceClient
        {
            get;
            private set;
        }

        public ConnectionServiceClientDecorator(ConnectionServiceClient connectionServiceClient)
        {
            ConnectionServiceClient = connectionServiceClient;
            ConnectionServiceClient.OnDisconnect = () => 
            {
                if (OnDisconnect != null)
                {
                    OnDisconnect();
                }
            };
        }

        private bool first = true;
        
        public void Init(Action<T> onDataReadyRead)
        {
            ConnectionServiceClient.Init(data =>
            {
                if (first)
                {
                    first = false;
                    Name = Encoding.UTF8.GetString(data);
                    if (OnNameReady != null)
                    {
                        OnNameReady();
                    }
                }
                else
                {
                    T obj = JsonUtils<T>.FromJson(Encoding.UTF8.GetString(data));
                    if (onDataReadyRead != null)
                    {
                        onDataReadyRead(obj);
                    }
                }
            });
        }

        public Action OnNameReady;

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

        public Action OnDisconnect
        {
            get;
            set;
        }

    }
}
