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
            DriverServiceOnClosing();
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
                    ResetBluetoothServer();
                }));
            };
            BluetoothController.Ondisconnect = ResetBluetoothServer;
            BluetoothController.OnDataReady = data =>
            {
                TransferObject obj = TransferObject.FromJson(Encoding.UTF8.GetString(data));
                if (obj?.Message == "DISCONNECT")
                {
                    BluetoothController.CloseConnect();
                    ResetBluetoothServer();
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

        private void ResetBluetoothServer()
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
                ConnectServerOnClosing();
                BluetoothController.Dispose();
                bluetoothBtnFlag = true;
                btnStartListener.Text = "开始监听";
                label3.Text = "状态: 未启动";
                ResetBluetoothServer();
            }
        }


        private void SendData(TransferObject obj)
        {
            BluetoothController?.Write(Encoding.UTF8.GetBytes(obj.ToJson()));
        }

        private void ConnectServerOnClosing()
        {
            try
            {
                SendData(new TransferObject(new Dictionary<string, object>(), 0, "DISCONNECT"));
            }
            catch (Exception e) { }
        }

        private void ConnectServerOnClose()
        {
            BluetoothController.Dispose();
        }

        #endregion

        #region 驱动服务

        private VirtualJoystickController JoystickController = VirtualJoystickController.GetInstance();
        private bool driverButtonFlag = true;
    
        private void InitDriverService()
        {
            lbManufacturer.Text = "厂商: null";
            lbProduct.Text = "产品: null";
        }

        private void btnInitDriver_Click(object sender, EventArgs e)
        {
            if (driverButtonFlag)
            {
                try
                {
                    JoystickController.InitJoystick();
                }catch(Exception ex)
                {
                    MessageBox.Show("请先安装ViGEmBus！地址：https://github.com/ViGEm/ViGEmBus");
                    return;
                }
                InitJoystickInformation();
                btnInitDriver.Text = "卸载驱动.";
                btnResetEvent.Enabled = true;
            }
            else
            {
                btnInitDriver.Text = "初始化驱动.";
                JoystickController.Dispose();
                btnResetEvent.Enabled = false;
            }
            driverButtonFlag = !driverButtonFlag;
        }

        private void InitJoystickInformation()
        {
            lbManufacturer.Text = "厂商: ViGEm";
            lbProduct.Text = "产品: Xbox 360";
        }

        public void OnEventReciver(Dictionary<string, object> data)
        {
            try
            {
                JoystickController.EventReciver(data ?? new Dictionary<string, object>());
            }
            catch (Exception e) { }
        }

        private void btnResetEvent_Click(object sender, EventArgs e)
        {
            JoystickController.ResetInput();
        }

        private void DriverServiceOnClosing()
        {
            JoystickController.Dispose();
        }

        #endregion


    }
}
