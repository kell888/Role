using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using KellRole;

namespace PermissionConfig
{
    public partial class UserForm : Form
    {
        public UserForm(List<User> data, List<Department> deps)
        {
            InitializeComponent();
            this.data = data;
            this.deps = deps;
        }

        List<User> data;

        public List<User> Data
        {
            get { return data; }
        }
        List<Department> deps;

        private void UserForm_Load(object sender, EventArgs e)
        {
            RefreshInfo();
        }

        private void RefreshInfo()
        {
            comboBox1.Items.Clear();
            comboBox2.Items.Clear();
            foreach (User d in data)
            {
                comboBox1.Items.Add(d);
            }
            foreach (Department d in deps)
            {
                comboBox2.Items.Add(d);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex > -1)
            {
                User d = comboBox1.SelectedItem as User;
                if (d != null)
                {
                    LoadInfo(d);
                }
            }
        }

        private void LoadInfo(User d)
        {
            textBox1.Text = d.Username;
            textBox2.Text = d.Remark;
            textBox3.Text = d.Password;
            for (int i = 0; i < comboBox2.Items.Count; i++)
            {
                Department dep = comboBox2.Items[i] as Department;
                if (dep != null)
                {
                    if (d.Departments.ContainsDepartment(dep))
                    {
                        comboBox2.SelectedIndex = i;
                        break;
                    }
                }
            }
            checkBox1.Checked = d.Flag == 1;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            User user = new User();
            user.Username = textBox1.Text.Trim();
            user.Remark = textBox2.Text;
            user.Flag = checkBox1.Checked ? 1 : 0;
            user.Password = textBox3.Text;
            Department dep = comboBox2.SelectedItem as Department;
            if (dep != null)
            {
                user.Departments.Clear();
                user.Departments.Add(dep);
            }
            UserLogic ul = UserLogic.GetInstance();
            if (ul.ExistsName(user.Username))
            {
                if (MessageBox.Show("系统中已经存在该名称，确定还要继续保存么？", "重名提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.OK)
                {
                    int id = ul.AddUser(user);
                    if (id > 0)
                    {
                        user.ID = id;
                        data.Add(user);
                        RefreshInfo();
                        MessageBox.Show("添加成功！");
                    }
                }
                else
                {
                    textBox1.Focus();
                    textBox1.SelectAll();
                }
            }
            else
            {
                int id = ul.AddUser(user);
                if (id > 0)
                {
                    user.ID = id;
                    data.Add(user);
                    RefreshInfo();
                    MessageBox.Show("添加成功！");
                }
            }
        }

        private void btn_User_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex > -1)
            {
                User user = new User();
                user.ID = data[comboBox1.SelectedIndex].ID;
                user.Username = textBox1.Text.Trim();
                user.Remark = textBox2.Text;
                user.Password = textBox3.Text;
                Department dep = comboBox2.SelectedItem as Department;
                if (dep != null)
                {
                    user.Departments.Clear();
                    user.Departments.Add(dep);
                }
                user.Flag = checkBox1.Checked ? 1 : 0;
                UserLogic ul = UserLogic.GetInstance();
                if (ul.ExistsNameOther(user.Username, user.ID))
                {
                    if (MessageBox.Show("系统中已经存在该名称，确定还要继续保存么？", "重名提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.OK)
                    {
                        if (ul.UpdateUser(user))
                        {
                            data[comboBox1.SelectedIndex].Username = user.Username;
                            data[comboBox1.SelectedIndex].Password = user.Password;
                            data[comboBox1.SelectedIndex].Flag = user.Flag;
                            data[comboBox1.SelectedIndex].Departments = user.Departments;
                            data[comboBox1.SelectedIndex].Remark = user.Remark;
                            RefreshInfo();
                            MessageBox.Show("修改成功！");
                        }
                    }
                    else
                    {
                        textBox1.Focus();
                        textBox1.SelectAll();
                    }
                }
                else
                {
                    if (ul.UpdateUser(user))
                    {
                        data[comboBox1.SelectedIndex].Username = user.Username;
                        data[comboBox1.SelectedIndex].Password = user.Password;
                        data[comboBox1.SelectedIndex].Flag = user.Flag;
                        data[comboBox1.SelectedIndex].Departments = user.Departments;
                        data[comboBox1.SelectedIndex].Remark = user.Remark;
                        RefreshInfo();
                        MessageBox.Show("修改成功！");
                    }
                }
            }
            else
            {
                MessageBox.Show("先选定要修改的项目！");
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex > -1)
            {
                if (MessageBox.Show("确定要删除该项目？", "删除提醒", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.OK)
                {
                    User user = data[comboBox1.SelectedIndex];
                    if (UserLogic.GetInstance().DeleteUser(user))
                    {
                        data.RemoveAt(comboBox1.SelectedIndex);
                        RefreshInfo();
                    }
                }
            }
            else
            {
                MessageBox.Show("先选定要删除的项目！");
            }
        }
    }
}
