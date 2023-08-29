using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZkTesting
{
    public partial class Form1 : Form
    {
        SDKHelper SDKHelper = new SDKHelper();

        public Form1()
        {
            InitializeComponent();
        }


        private void button_Connect_Click(object sender, EventArgs e)
        {
            SDKHelper.sta_ConnectTCP();

        }

        private void button_AddUser_Click(object sender, EventArgs e)
        {
            var x = SDKHelper.sta_OnlineEnroll();

        }

        private void button_SetAccessGroup_Click(object sender, EventArgs e)
        {
            SDKHelper.setGroupTZ(1, 1, 2, 3);
        }

        private void button_SetUserAccess_Click(object sender, EventArgs e)
        {
            SDKHelper.setUserAccessGroup(Convert.ToInt32(textBox1.Text), Convert.ToInt32(textBox2.Text));
        }
        private void button_getUserAccess_Click(object sender, EventArgs e)
        {
            SDKHelper.getUserAccessGroup(Convert.ToInt32(textBox1.Text));
        }
    }
}
