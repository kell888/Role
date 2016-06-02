using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using KellRole;

namespace PermissionConfig
{
    public partial class ConfigClient : Form
    {
        const int CLOSE_SIZE = 12;
        private ConfigClient()
        {
            InitializeComponent();
            archit = new Architecture();
            this.tabControl1.DrawMode = TabDrawMode.OwnerDrawFixed;//由自己绘制标题
            this.tabControl1.Padding = new System.Drawing.Point(CLOSE_SIZE + 2, 2);
            this.tabControl1.DrawItem += new DrawItemEventHandler(this.tabControl1_DrawItem);
            this.tabControl1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tabControl1_MouseDown);
        }
        static ConfigClient instance;

        public static ConfigClient GetInstance()
        {
            if (instance == null || instance.IsDisposed)
            {
                instance = new ConfigClient();
            }
            if (!instance.Visible)
                instance.Show();
            instance.WindowState = FormWindowState.Maximized;
            instance.BringToFront();
            instance.Focus();
            return instance;
        }
        private bool sure;
        private Architecture archit;

        public User User
        {
            get { return UserLogic.GetInstance().GetUser(Common.AdminId); }
        }

        #region 判断TabPage是否已创建，添加TabPage
        internal void AddTabPage(string str, Form form)
        {
            int have = TabControlCheckHave(this.tabControl1, form.GetType().FullName);
            if (have > -1)
            {
                tabControl1.SelectTab(have);
                tabControl1.SelectedTab.Text = str;
            }
            else
            {
                tabControl1.TabPages.Add(str);
                tabControl1.SelectTab(tabControl1.TabPages.Count - 1);
                tabControl1.SelectedTab.Tag = form.GetType().FullName;
                tabControl1.SelectedTab.Name = "Tab_" + form.Name;
                form.TopLevel = false;//设置非顶级窗口
                form.Parent = tabControl1.SelectedTab;
                form.WindowState = FormWindowState.Maximized;
                form.FormBorderStyle = FormBorderStyle.None;
                form.Show();
            }
        }
        //判断TabPage是否已创建
        private int TabControlCheckHave(TabControl tab, string tabPage)
        {
            int index = -1;
            for (int i = 0; i < tab.TabPages.Count; i++)
            {
                TabPage tp = tab.TabPages[i];
                if (tp.Tag.ToString() == tabPage)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }
        #endregion
        #region tabpage页面的关闭按钮及事件
        private void tabControl1_DrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e)
        {
            try
            {
                Rectangle myTabRect = this.tabControl1.GetTabRect(e.Index);
                
                //先添加TabPage属性   
                e.Graphics.DrawString(this.tabControl1.TabPages[e.Index].Text
                , this.Font, SystemBrushes.ControlText, myTabRect.X + 2, myTabRect.Y + 2);

                //再画一个矩形框
                using (Pen p = new Pen(Color.Black))
                {
                    myTabRect.Offset(myTabRect.Width - (CLOSE_SIZE + 3), 2);
                    myTabRect.Width = CLOSE_SIZE;
                    myTabRect.Height = CLOSE_SIZE;
                    e.Graphics.DrawRectangle(p, myTabRect);
                }

                //填充矩形框
                Color recColor = e.State == DrawItemState.Selected ? Color.MediumVioletRed : Color.DarkGray;
                using (Brush b = new SolidBrush(recColor))
                {
                    e.Graphics.FillRectangle(b, myTabRect);
                }

                //画关闭符号
                using (Pen p = new Pen(Color.White))
                {
                    //画"\"线
                    Point p1 = new Point(myTabRect.X + 3, myTabRect.Y + 3);
                    Point p2 = new Point(myTabRect.X + myTabRect.Width - 3, myTabRect.Y + myTabRect.Height - 3);
                    e.Graphics.DrawLine(p, p1, p2);

                    //画"/"线
                    Point p3 = new Point(myTabRect.X + 3, myTabRect.Y + myTabRect.Height - 3);
                    Point p4 = new Point(myTabRect.X + myTabRect.Width - 3, myTabRect.Y + 3);
                    e.Graphics.DrawLine(p, p3, p4);
                }

                e.Graphics.Dispose();
            }
            catch (Exception)
            {

            }
        }
        private void tabControl1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int x = e.X, y = e.Y;

                //计算关闭区域   
                Rectangle myTabRect = this.tabControl1.GetTabRect(this.tabControl1.SelectedIndex);

                myTabRect.Offset(myTabRect.Width - (CLOSE_SIZE + 3), 2);
                myTabRect.Width = CLOSE_SIZE;
                myTabRect.Height = CLOSE_SIZE;

                //如果鼠标在区域内就关闭选项卡   
                bool isClose = x > myTabRect.X && x < myTabRect.Right
                 && y > myTabRect.Y && y < myTabRect.Bottom;

                if (isClose)
                {
                    string name = this.tabControl1.SelectedTab.Name.Substring(4);
                    Control[] cs = this.tabControl1.SelectedTab.Controls.Find(name, false);
                    if (cs != null && cs.Length > 0)
                    {
                        if (cs[0] is Form)
                        {
                            Form f = cs[0] as Form;
                            f.Close();
                        }
                    }
                    this.tabControl1.TabPages.Remove(this.tabControl1.SelectedTab);
                }
            }
        }

        #endregion

        private void ConfigClient_Load(object sender, EventArgs e)
        {
            LockForm f8 = new LockForm(this);
            if (f8.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.panel2.SendToBack();
                this.panel2.Hide();
            }
            else
            {
                notifyIcon1.Dispose();
                Environment.Exit(0);
            }
        }

        private void LoadArchitecture(Architecture a)
        {
            if (a != null)
            {
                if (a.Users != null)
                    archit.Users = a.Users;
                if (a.Ugroups != null)
                    archit.Ugroups = a.Ugroups;
                if (a.Deps != null)
                    archit.Deps = a.Deps;
                if (a.Pers != null)
                    archit.Pers = a.Pers;
                if (a.Roles != null)
                    archit.Roles = a.Roles;
                if (a.Mods != null)
                    archit.Mods = a.Mods;
                if (a.Acts != null)
                    archit.Acts = a.Acts;
            }
        }
        #region 以下是多余的
        //private void RefreshArchitecture_Users(List<User> users)
        //{
        //    if (users != null)
        //        archit.Users = users;
        //}

        //private void RefreshArchitecture_UserGroups(List<UserGroup> ugrps)
        //{
        //    if (ugrps != null)
        //        archit.Ugroups = ugrps;
        //}

        //private void RefreshArchitecture_Departments(List<Department> deps)
        //{
        //    if (deps != null)
        //        archit.Deps = deps;
        //}

        //private void RefreshArchitecture_Permissions(List<Permission> pers)
        //{
        //    if (pers != null)
        //        archit.Pers = pers;
        //}

        //private void RefreshArchitecture_Roles(List<Role> roles)
        //{
        //    if (roles != null)
        //        archit.Roles = roles;
        //}

        //private void RefreshArchitecture_Modules(List<Module> mods)
        //{
        //    if (mods != null)
        //        archit.Mods = mods;
        //}

        //private void RefreshArchitecture_Actions(List<Action> acts)
        //{
        //    if (acts != null)
        //        archit.Acts = acts;
        //}
#endregion
        internal void LoadArchitectureFromLocal()
        {
            openFileDialog1.Title = "载入指定的架构信息";
            openFileDialog1.Filter = "架构信息|*.ainf";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Architecture a = null;
                using (FileStream fs = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    a = (Architecture)bf.Deserialize(fs);
                }
                LoadArchitecture(a);
            }
            openFileDialog1.Dispose();
        }

        internal void SaveArchitectureToLocal()
        {
            saveFileDialog1.Title = "保存当前架构信息";
            saveFileDialog1.Filter = "架构信息|*.ainf";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                using (FileStream fs = new FileStream(saveFileDialog1.FileName, FileMode.Create, FileAccess.Write))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, this.CurrentArchitecture);
                    MessageBox.Show("架构信息保存成功！");
                }
            }
            saveFileDialog1.Dispose();
        }

        private static Architecture GetRemoteArchitecture()
        {
            Architecture a = Architecture.Empty;
            a.Deps = DepartmentLogic.GetInstance().GetAllDepartments();
            a.Ugroups = UserGroupLogic.GetInstance().GetAllUserGroups();
            a.Users = UserLogic.GetInstance().GetAllUsers();
            a.Mods = ModuleLogic.GetInstance().GetAllModules();
            a.Acts = ActionLogic.GetInstance().GetAllActions();
            a.Pers = PermissionLogic.GetInstance().GetAllPermissions();
            a.Roles = RoleLogic.GetInstance().GetAllRoles();
            return a;
        }

        private void ConfigClient_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (sure)
            {
                if (MessageBox.Show("确定要退出程序吗？", "退出提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.OK)
                {
                    //判断是否架构配置界面没有退出，以便提示保存
                    foreach (Form f in Application.OpenForms)
                    {
                        if (f.GetType() == typeof(AssignForm))
                        {
                            if (MessageBox.Show("是否已经改动架构？如果已经改动，请更新架构到远程服务器，以免丢失！", "保存提醒", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.OK)
                            {
                                Exception ex;
                                if (UpgradeArchitectureToRemote(out ex))
                                {
                                    MessageBox.Show("更新成功！");
                                }
                                else
                                {
                                    MessageBox.Show("更新失败：" + ex.Message);
                                }
                            }
                        }
                    }
                    notifyIcon1.Dispose();
                    Environment.Exit(0);
                }
                else
                {
                    e.Cancel = true;
                    sure = false;
                }
            }
            else
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        private void Exit()
        {
            sure = true;
            this.Close();
        }

        private void 退出ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Exit();
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Exit();
        }

        private void ShowUI()
        {
            if (this.WindowState == FormWindowState.Minimized)
                this.WindowState = FormWindowState.Normal;
            this.Show();
            this.BringToFront();
        }

        private void 显示ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowUI();
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            ShowUI();
        }

        private void 锁定ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LockSystem();
        }

        private void 锁定ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            LockSystem();
        }

        private void LockSystem()
        {
            this.textBox1.Clear();
            this.textBox1.Focus();
            this.textBox1.SelectAll();
            this.WindowState = FormWindowState.Maximized;
            this.BringToFront();
            this.Show();
            this.panel2.Show();
            this.panel2.BringToFront();
            DisableMenus();
        }

        private void UnlockSystem()
        {
            this.panel2.SendToBack();
            this.panel2.Hide();
            EnableMenus();
        }

        private void DisableMenus()
        {
            foreach (ToolStripMenuItem c in menuStrip1.Items)
            {
                if (c.Name != "锁定ToolStripMenuItem")
                    c.Enabled = false;
            }
            foreach (ToolStripItem c in contextMenuStrip1.Items)
            {
                if (c.Name != "锁定ToolStripMenuItem1")
                    c.Enabled = false;
            }
        }

        private void EnableMenus()
        {
            foreach (ToolStripMenuItem c in menuStrip1.Items)
            {
                if (c.Name != "锁定ToolStripMenuItem")
                    c.Enabled = true;
            }
            foreach (ToolStripItem c in contextMenuStrip1.Items)
            {
                if (c.Name != "锁定ToolStripMenuItem1")
                    c.Enabled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {//解锁
            Unlock(textBox1);
        }

        public bool Unlock(TextBox textBox1)
        {
            string auth = textBox1.Text.ToLower();
            if (auth != string.Empty)
            {
                if (auth.Length >= 8)
                {
                    if (auth == DateTime.Now.ToString("yyMMddHH") + CommonConsts.Auth.ToLower())
                    {
                        UnlockSystem();
                        ThreadPool.QueueUserWorkItem(delegate
                        {
                            int a = Common.AdminId;
                        });
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("授权码错误！");
                        textBox1.SelectAll();
                        textBox1.Focus();
                    }
                }
                else
                {
                    MessageBox.Show("授权码长度有误！");
                    textBox1.SelectAll();
                    textBox1.Focus();
                }
            }
            else
            {
                MessageBox.Show("请输入授权码！");
                textBox1.Focus();
            }
            return false;
        }

        internal void DownloadRemoteArchitecture()
        {
            SetStatusText("正在从远处服务器下载架构信息，请稍候...");
            Architecture a = GetRemoteArchitecture();
            LoadArchitecture(a);
            SetStatusText("Ready...");
        }

        internal void SetStatusText(string text)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate
                {
                    toolStripStatusLabel1.Text = text;
                    statusStrip1.Refresh();
                }));
            }
            else
            {
                toolStripStatusLabel1.Text = text;
                statusStrip1.Refresh();
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
                Unlock(textBox1);
        }

        public static DataTable LoadTableFromFile(string filename)
        {
            DataTable data = new DataTable();
            try
            {
                XmlReadMode xrm = data.ReadXml(filename);
            }
            catch (Exception e)
            {
                MessageBox.Show("读取文件出错：" + e.Message);
            }
            return data;
        }

        public static bool SaveTableToFile(DataTable data, string filename)
        {
            if (data == null || data.Columns.Count == 0)
                return false;
            if (string.IsNullOrEmpty(data.TableName))
                data.TableName = "数据";
            try
            {
                data.WriteXml(filename, XmlWriteMode.WriteSchema);
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("写入文件出错：" + e.Message);
                return false;
            }
        }

        private void 部门管理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DepartForm depForm = new DepartForm(archit.Deps);
            AddTabPage("部门管理", depForm);
        }

        private void 用户组管理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UgForm ugForm = new UgForm(archit.Ugroups);
            AddTabPage("用户组管理", ugForm);
        }

        private void 用户管理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UserForm uForm = new UserForm(archit.Users, archit.Deps);
            AddTabPage("用户管理", uForm);
        }

        private void 模块管理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ModForm mForm = new ModForm(archit.Mods);
            AddTabPage("模块管理", mForm);
        }

        private void 操作管理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ActForm aForm = new ActForm(archit.Acts);
            AddTabPage("操作管理", aForm);
        }

        private void 权限管理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PerForm pForm = new PerForm(archit.Pers, archit.Mods, archit.Acts);
            AddTabPage("权限管理", pForm);
        }

        private void 角色管理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RoleForm rForm = new RoleForm(archit.Roles);
            AddTabPage("角色管理", rForm);
        }

        internal Architecture CurrentArchitecture
        {
            get
            {
                return archit;
            }
        }

        private void 架构配置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AssignForm assForm = new AssignForm(this);
            AddTabPage("架构配置", assForm);
        }

        private void 保存架构ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Exception ex;
            if (UpgradeArchitectureToRemote(out ex))
                MessageBox.Show("更新成功！");
            else
                MessageBox.Show("更新失败："+ ex.Message);
        }

        internal bool UpgradeArchitectureToRemote(out Exception ex)
        {
            ex = null;
            int errCount = 0;
            try
            {
                Architecture a = this.CurrentArchitecture;
                List<KellRole.Action> atcs = a.Acts;
                List<Department> deps = a.Deps;
                List<Module> mods = a.Mods;
                List<Permission> perms = a.Pers;
                List<Role> roles = a.Roles;
                List<UserGroup> ugrps = a.Ugroups;
                List<User> users = a.Users;
                ActionLogic.GetInstance().UpgradeList(atcs);
                DepartmentLogic.GetInstance().UpgradeList(deps);
                ModuleLogic.GetInstance().UpgradeList(mods);
                PermissionLogic.GetInstance().UpgradeList(perms);
                RoleLogic.GetInstance().UpgradeList(roles);
                UserGroupLogic.GetInstance().UpgradeList(ugrps);
                UserLogic.GetInstance().UpgradeList(users);
            }
            catch (Exception e)
            {
                errCount++;
                ex = e;
            }
            return errCount == 0;
        }
    }
}
