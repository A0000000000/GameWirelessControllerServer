using ConnectionServiceServer.Utils;
using InTheHand.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConnectionServiceServer.Internal
{
    public class ConnectionServiceClient: IDisposable
    {

        public BluetoothClient BluetoothClient
        {
            get;
            private set;
        }

        public ConnectionServiceClient(BluetoothClient client)
        {
            BluetoothClient = client;
        }

        private NetworkStream stream;
        private Thread readThread;

        public void Init(Action<byte[]> onDataRead)
        {
            stream = BluetoothClient.GetStream();
            readThread = new Thread(() => 
            {
                while (true)
                {
                    byte[] length = new byte[4];
                    stream.Read(length, 0, length.Length);
                    int size = ByteLengthUtils.GetLengthInteger(length);
                    if (size == 0)
                    {
                        if (OnDisconnect != null)
                        {
                            OnDisconnect();
                        }
                        break;
                    }
                    byte[] data = new byte[size];
                    stream.Read(data, 0, data.Length);
                    if (onDataRead != null)
                    {
                        onDataRead(data);
                    }
                }
            });
            readThread.Start();
        }

        public void Write(byte[] data)
        {
            if (data != null)
            {
                byte[] size = ByteLengthUtils.GetLengthArray(data.Length);
                stream.Write(size, 0, size.Length);
                stream.Write(data, 0, data.Length);
                stream.Flush();
            }
        }

        public void Dispose()
        {
            stream?.Dispose();
            stream = null;
        }

        public Action OnDisconnect
        {
            get;
            set;
        }

    }
}
