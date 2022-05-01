using ConnectionServiceServer.Utils;
using InTheHand.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConnectionServiceServer.Raw
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
        private Action<Exception> OnConnectionException;

        public void Init(Action<byte[]> onDataRead, Action<Exception> onConnectionException)
        {
            OnConnectionException = onConnectionException;
            stream = BluetoothClient.GetStream();
            readThread = new Thread(() => 
            {
                while (true)
                {
                    try
                    {
                        byte[] length = new byte[4];
                        for (int i = 0; i < length.Length; ++i)
                        {
                            length[i] = (byte)stream.ReadByte();
                            if (length[i] < 0)
                            {
                                throw new Exception("Normal Exit.");
                            }
                        }
                        int size = ByteLengthUtils.GetLengthInteger(length);
                        if (size != 0)
                        {
                            byte[] data = new byte[size];
                            for (int i = 0; i < size; ++i)
                            {
                                data[i] = (byte)stream.ReadByte();
                                if (data[i] < 0)
                                {
                                    throw new Exception("Normal Exit.");
                                }
                            }
                            if (onDataRead != null)
                            {
                                Console.WriteLine("IO: IN begin");
                                Console.WriteLine("IO: size = " + size);
                                Console.WriteLine("IO: data = [" + string.Join(", ", data) + "]");
                                Console.WriteLine("IO: IN end");
                                onDataRead(data);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        if (OnConnectionException != null)
                        {
                            OnConnectionException(e);
                        }
                        Console.WriteLine(e.Message);
                        break;
                    }
                }
            });
            readThread.Start();
        }

        public void Write(byte[] data)
        {
            try
            {
                if (data != null && data.Length != 0)
                {
                    Console.WriteLine("IO: OUT begin");
                    Console.WriteLine("IO: size = " + data.Length);
                    Console.WriteLine("IO: data = [" + string.Join(", ", data) + "]");
                    Console.WriteLine("IO: OUT end");
                    byte[] size = ByteLengthUtils.GetLengthArray(data.Length);
                    stream.Write(size, 0, size.Length);
                    stream.Write(data, 0, data.Length);
                    stream.Flush();
                }
            }
            catch (Exception e)
            {
                if (OnConnectionException != null)
                {
                    OnConnectionException(e);
                }
                Console.WriteLine(e.Message);
            }
        }

        public void Dispose()
        {
            BluetoothClient.Dispose();
        }

    }
}
