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

        private GameWirelessController Controller;
        private bool flag = true;
        
        public FormMain()
        {
            InitializeComponent();
        }
        private void FormMain_Load(object sender, EventArgs e)
        {
            button2.Enabled = false;
        }
        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            Controller?.Dispose();
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (flag)
            {
                label2.Text = "已启动";
                Controller = new GameWirelessController();
                Controller.Init();
                flag = false;
                button1.Enabled = false;
                button2.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!flag)
            {
                label2.Text = "未启动";
                Controller?.Dispose();
                Controller = null;
                flag = true;
                button2.Enabled = false;
                button1.Enabled = true;
            }
        }
    }
}
