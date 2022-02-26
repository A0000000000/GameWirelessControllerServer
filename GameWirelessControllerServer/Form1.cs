using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameWirelessControllerServer
{
    public partial class FormMain : Form
    {

        private BluetoothServerController Controller = BluetoothServerController.GetInstance();

        public bool buttonFlag = true;
        public FormMain()
        {
            InitializeComponent();
            Controller.OnDeviceConnected = () =>
            {
                string name = Controller.getDeviceName();
                string address = Controller.getDeviceAddress();
                BeginInvoke(new MethodInvoker(() =>
                {
                    label_name.Text = name;
                    label_address.Text = address;
                }));
            };
            Controller.OnListenerQuit = () =>
            {
                BeginInvoke(new MethodInvoker(() =>
                {
                    buttonFlag = true;
                    btnStartListener.Text = "开始监听";
                    label3.Text = "状态: 未启动";
                    Reset();
                }));
            };
            Controller.Ondisconnect = Reset;
            Controller.OnDataReady = data =>
            {
                TransferObject obj = TransferObject.FromJson(Encoding.UTF8.GetString(data));
                if (obj?.Message == "DISCONNECT")
                {
                    Controller.CloseConnect();
                    Reset();
                    return;
                }
                else
                {
                    SendData(new TransferObject(new Dictionary<string, object>(), 0, "SUCCESS"));
                }
                BeginInvoke(new MethodInvoker(() =>
                {
                    label_event.Text = obj.ToString();
                }));
            };
        }

        private void Reset()
        {
            BeginInvoke(new MethodInvoker(() =>
            {
                label_name.Text = "No Name";
                label_address.Text = "No Address";
                label_event.Text = "No Event";
            }));
        }

        private void btnStartListener_Click(object sender, EventArgs e)
        {
            if (buttonFlag)
            {
                buttonFlag = false;
                btnStartListener.Text = "停止监听";
                label3.Text = "状态: 已启动";
                Controller.StartListener();
            }
            else 
            {
                Controller.Dispose();
                buttonFlag = true;
                btnStartListener.Text = "开始监听";
                label3.Text = "状态: 未启动";
                Reset();
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            SendData(new TransferObject(new Dictionary<string, object>(), 0, "DISCONNECT"));
        }


        private void SendData(TransferObject obj)
        {
            Controller?.Write(Encoding.UTF8.GetBytes(obj.ToJson()));
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            Controller.Dispose();
            Application.Exit();
        }
    }
}
