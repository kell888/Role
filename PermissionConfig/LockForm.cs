using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PermissionConfig
{
    public partial class LockForm : Form
    {
        public LockForm(ConfigClient owner)
        {
            InitializeComponent();
            this.owner = owner;
        }

        ConfigClient owner;

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button1.Text = "登录中...";
            button1.Refresh();
            if (owner.Unlock(textBox1))
            {
                try
                {
                    owner.DownloadRemoteArchitecture();
                    AssignForm aForm = new AssignForm(owner);
                    owner.AddTabPage("组织架构配置", aForm);
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                }
                catch
                {
                    button1.Enabled = true;
                    button1.Text = "登  录";
                    button1.Refresh();
                }
            }
            else
            {
                button1.Enabled = true;
                button1.Text = "登  录";
                button1.Refresh();
            }
        }
    }
}
