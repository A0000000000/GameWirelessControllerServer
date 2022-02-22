using InTheHand.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameWirelessControllerServer
{
    class BluetoothServerController
    {

        public static readonly string GUID = "6ef82393-6cab-4749-b0b5-df0109fb7dec";

        private static BluetoothServerController INSTANCE;

        private static readonly object LOCK = new object();

        public static BluetoothServerController GetInstance()
        {
            if (INSTANCE == null)
            {
                lock(LOCK)
                {
                    if (INSTANCE == null)
                    {
                        INSTANCE = new BluetoothServerController();
                    }
                }
            }
            return INSTANCE;
        }

        private BluetoothServerController()
        {
            Listener = new BluetoothListener(Guid.Parse(GUID));
        }

        public Thread ListenerThread;

        public BluetoothListener Listener
        {
            get;
            set;
        }

        public BluetoothClient Client
        {
            get;
            set;
        }

        public NetworkStream Stream
        {
            get;
            set;
        }

        public void StartListener()
        {
            ListenerThread = new Thread(() =>
            {
                Listener.Start();
                while (true)
                {
                    try
                    {
                        Client = Listener.AcceptBluetoothClient();
                        try
                        {
                            Stream = Client.GetStream();
                            while (Client.Connected)
                            {
                                byte[] len = new byte[4];
                                int size = 0;
                                Stream.Read(len, 0, 4);
                                for (int i = 0; i < 3; ++i)
                                {
                                    size |= len[i];
                                    size <<= 8;
                                }
                                size |= len[3];
                                byte[] data = new byte[size];
                                Stream.Read(data, 0, size);
                                string str = Encoding.UTF8.GetString(data).ToString();
                                if (OnDataReady != null)
                                {
                                    OnDataReady(str);
                                }
                                Write("SUCCESS");
                            }
                        }
                        catch (Exception ignore) { }
                        finally
                        {
                            Stream = null;
                            Client = null;
                        }

                    }
                    catch (Exception e)
                    {
                        // MessageBox.Show($"出现错误, 原因: {e.Message}");
                        break;
                    }
                }
                if (OnListenerQuit != null)
                {
                    OnListenerQuit();
                }
            });
            ListenerThread.Start();
        }

        public Action OnListenerQuit
        {
            get;
            set;
        }

        public Action<string> OnDataReady;


        public void Write(string data)
        {
            if (data != null && Stream != null)
            {
                byte[] send = Encoding.UTF8.GetBytes(data);
                byte[] len =  { (byte)(send.Length >> 24), (byte)(send.Length >> 16), (byte)(send.Length >> 8), (byte)(send.Length) };
                Stream.Write(len, 0, 4);
                Stream.Write(send, 0, send.Length);
                Stream.Flush();
            }
        }

        public string getDeviceName()
        {
            if (Client != null)
            {
                return Client.RemoteMachineName;
            }
            return "No Name";
        }

        public string getDeviceAddress()
        {
            if (Client != null)
            {
                return Client.Client.RemoteEndPoint.ToString();
            }
            return "No Address";
        }

        public void Dispose()
        {
            try
            {
                Listener.Stop();
                ListenerThread.Abort();
                Stream.Close();
                Client.Dispose();
            }
            catch(Exception e)
            {

            }
            finally
            {
                ListenerThread = null;
                Stream = null;
                Client = null;
            }
        }

    }
}
