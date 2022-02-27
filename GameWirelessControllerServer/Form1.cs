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

        
        public FormMain()
        {
            InitializeComponent();

            InitConnectServer();
            InitDriverService();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            ConnectServerOnClosing();
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            ConnectServerOnClose();
            Application.Exit();
        }

        #region 通讯服务

        private BluetoothServerController BluetoothController = BluetoothServerController.GetInstance();

        public bool bluetoothBtnFlag = true;

        private void InitConnectServer()
        {
            BluetoothController.OnDeviceConnected = () =>
            {
                string name = BluetoothController.getDeviceName();
                string address = BluetoothController.getDeviceAddress();
                BeginInvoke(new MethodInvoker(() =>
                {
                    label_name.Text = name;
                    label_address.Text = address;
                }));
            };
            BluetoothController.OnListenerQuit = () =>
            {
                BeginInvoke(new MethodInvoker(() =>
                {
                    bluetoothBtnFlag = true;
                    btnStartListener.Text = "开始监听";
                    label3.Text = "状态: 未启动";
                    Reset();
                }));
            };
            BluetoothController.Ondisconnect = Reset;
            BluetoothController.OnDataReady = data =>
            {
                TransferObject obj = TransferObject.FromJson(Encoding.UTF8.GetString(data));
                if (obj?.Message == "DISCONNECT")
                {
                    BluetoothController.CloseConnect();
                    Reset();
                    return;
                }
                else
                {
                    SendData(new TransferObject(new Dictionary<string, object>(), 0, "SUCCESS"));
                }
                if (obj?.Message == "EVENT")
                {
                    OnEventReciver(obj?.Data ?? new Dictionary<string, object>());
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
            if (bluetoothBtnFlag)
            {
                bluetoothBtnFlag = false;
                btnStartListener.Text = "停止监听";
                label3.Text = "状态: 已启动";
                BluetoothController.StartListener();
            }
            else 
            {
                BluetoothController.Dispose();
                bluetoothBtnFlag = true;
                btnStartListener.Text = "开始监听";
                label3.Text = "状态: 未启动";
                Reset();
            }
        }


        private void SendData(TransferObject obj)
        {
            BluetoothController?.Write(Encoding.UTF8.GetBytes(obj.ToJson()));
        }

        private void ConnectServerOnClosing()
        {
            SendData(new TransferObject(new Dictionary<string, object>(), 0, "DISCONNECT"));
        }

        private void ConnectServerOnClose()
        {
            BluetoothController.Dispose();
        }

        #endregion

        #region 驱动服务

        private VirtualJoystickController JoystickController = VirtualJoystickController.GetInstance();
    
        private void InitDriverService()
        {
            lbManufacturer.Text = "厂商: null";
            lbProduct.Text = "产品: null";
            lbDriverVer.Text = "驱动版本: null";
            lbDllVer.Text = "库版本: null";
        }

        private void btnInitDriver_Click(object sender, EventArgs e)
        {
            if (!JoystickController.Enable())
            {
                MessageBox.Show("vJoy驱动未安装!");
                return;
            }
            if (!JoystickController.CheckConfiguration())
            {
                MessageBox.Show("配置错误,请检查vJoy的配置！");
            }
            JoystickController.InitJoystick();
            InitJoystickInformation();
            btnInitDriver.Enabled = false;
            btnResetEvent.Enabled = true;
        }

        private void InitJoystickInformation()
        {
            lbManufacturer.Text = "厂商: " + JoystickController.GetvJoyManufacturer();
            lbProduct.Text = "产品: " + JoystickController.GetvJoyProduct();
            uint drvVer = 0, dllVer = 0;
            JoystickController.DriverMatch(ref dllVer, ref drvVer);
            lbDriverVer.Text = "驱动版本: " + drvVer;
            lbDllVer.Text = "库版本: " + dllVer;
        }

        public void OnEventReciver(Dictionary<string, object> data)
        {
            JoystickController.EventReciver(data ?? new Dictionary<string, object>());
        }

        private void btnResetEvent_Click(object sender, EventArgs e)
        {
            JoystickController.ResetInput();
        }

        #endregion


    }
}
