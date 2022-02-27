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

        private static BluetoothServerController INSTANCE = null;

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
                try
                {
                    Listener.Start();
                }
                catch (Exception e) 
                {
                    if (OnListenerQuit != null)
                    {
                        OnListenerQuit();
                    }
                    return;
                }
                while (true)
                {
                    try
                    {
                        Client = Listener.AcceptBluetoothClient();
                        if (OnDeviceConnected != null)
                        {
                            OnDeviceConnected();
                        }
                        try
                        {
                            Stream = Client.GetStream();
                            while (Client?.Connected ?? false)
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
                                if (OnDataReady != null)
                                {
                                    OnDataReady(data);
                                }
                            }
                        }
                        catch (Exception e) 
                        {
                        }
                        finally
                        {
                            try
                            {
                                CloseConnect();
                            }
                            catch (Exception ignore) { }
                        }
                    }
                    catch (Exception e)
                    {
                        break;
                    }
                }
                try
                {
                    Dispose();
                }
                catch (Exception ignore) { }
                if (OnListenerQuit != null)
                {
                    OnListenerQuit();
                }
            });
            ListenerThread.Start();
        }

        public Action OnDeviceConnected
        {
            get;
            set;
        }

        public Action OnListenerQuit
        {
            get;
            set;
        }

        public Action Ondisconnect
        {
            get;
            set;
        }

        public Action<byte[]> OnDataReady;


        public void Write(byte[] send)
        {
            if (send != null && Stream != null)
            {
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

        public void CloseConnect()
        {
            try
            {
                if (Ondisconnect != null)
                {
                    Ondisconnect();
                }
                Stream?.Close();
                Client?.Dispose();
            }
            catch (Exception e)
            {

            }
            finally
            {
                Stream = null;
                Client = null;
            }
        }

        public void Dispose()
        {
            try
            {
                CloseConnect();
                Listener?.Stop();
                ListenerThread?.Abort();
            }
            catch(Exception e)
            {

            }
            finally
            {
                ListenerThread = null;
            }
        }

    }
}
