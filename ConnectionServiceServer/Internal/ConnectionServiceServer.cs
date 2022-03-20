using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InTheHand.Net.Sockets;


namespace ConnectionServiceServer.Internal
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
                    BluetoothClient client = BluetoothListener.AcceptBluetoothClient();
                    if (onClientConnected != null)
                    {
                        onClientConnected(new ConnectionServiceClient(client));
                    }
                }
            });
            listenerThread.Start();
        }

        public void Dispose()
        {
            listenerThread.Abort();
            BluetoothListener.Dispose();
        }

    }
}
