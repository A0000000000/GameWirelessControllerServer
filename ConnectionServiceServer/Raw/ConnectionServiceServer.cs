using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InTheHand.Net.Sockets;


namespace ConnectionServiceServer.Raw
{
    public class ConnectionServiceServer: IDisposable
    {
        public Guid Guid
        {
            get;
            private set;
        }

        public BluetoothListener BluetoothListener
        {
            get;
            private set;
        }

        public ConnectionServiceServer(string guid)
        {
            Guid = Guid.Parse(guid);
            BluetoothListener = new BluetoothListener(Guid);
        }

        private Thread listenerThread;

        public void Init(Action<ConnectionServiceClient> onClientConnected)
        {
            BluetoothListener.Start();
            listenerThread = new Thread(() => {
                while (true)
                {
                    try
                    {
                        BluetoothClient client = BluetoothListener.AcceptBluetoothClient();
                        if (onClientConnected != null)
                        {
                            onClientConnected(new ConnectionServiceClient(client));
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        break;
                    }
                }
            });
            listenerThread.Start();
        }

        public void Dispose()
        {
            BluetoothListener.Dispose();
        }

    }
}
