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

        public int buttonFlag = 2;
        public FormMain()
        {
            InitializeComponent();
        }

        private void btnStartListener_Click(object sender, EventArgs e)
        {
            if (buttonFlag >= 2)
            {
                buttonFlag--;
                btnStartListener.Text = "停止监听";
                label3.Text = "状态: 已启动";
                Controller.OnDataReady = data =>
                {
                    BeginInvoke(new MethodInvoker(() =>
                    {
                        if (buttonFlag >= 1)
                        {
                            buttonFlag--;
                            label_name.Text = Controller.getDeviceName();
                            label_address.Text = Controller.getDeviceAddress();
                        }
                        label_event.Text = data;
                    }));
                    
                };
                Controller.OnListenerQuit = () =>
                {
                    BeginInvoke(new MethodInvoker(() =>
                    {
                        buttonFlag = 2;
                        btnStartListener.Text = "开始监听";
                        label3.Text = "状态: 未启动";
                    }));
                };
                Controller.StartListener();
            }
            else 
            {
                Controller.Dispose();
                buttonFlag = 2;
                btnStartListener.Text = "开始监听";
                label3.Text = "状态: 未启动";
                label_name.Text = "No Name";
                label_address.Text = "No Address";
                label_event.Text = "No Event";
            }
        }
    }
}
