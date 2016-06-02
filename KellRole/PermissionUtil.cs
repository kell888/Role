using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace KellRole
{
    public interface IPermission
    {
        User User { get; set; }
        int AdminId { get; }
        void CheckUserPermission(Form child);
        void DisableUserPermission(Form child);
    }

    public class PermissionForm : Form, IPermission
    {
        private User user;
        /// <summary>
        /// 当前用户
        /// </summary>
        [Browsable(false)]
        public User User
        {
            get { return user; }
            set { user = value; }
        }
        /// <summary>
        /// 管理员ID
        /// </summary>
        public int AdminId
        {
            get
            {
                return Common.AdminId;
            }
        }
        /// <summary>
        /// 用于主窗体的包含权限
        /// </summary>
        /// <param name="child"></param>
        public void CheckUserPermission(Form child)
        {
            child.EnableChildrenForUser();
        }
        /// <summary>
        /// 用于子窗体的排除权限
        /// </summary>
        /// <param name="child"></param>
        public void DisableUserPermission(Form child)
        {
            child.DisableForUser();
        }
    }

    internal static class ControlExtensions
    {
        /// <summary>
        /// 用于子窗体的排除权限
        /// </summary>
        /// <param name="control"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        internal static bool DisableForUser(this Control control, User user = null)
        {
            return Disable(control, user);
        }

        private static bool Disable(Control control, User user)
        {
            Form owner = control.FindForm();
            if (owner != null)
            {
                if (user == null)
                {
                    PermissionForm pf = owner as PermissionForm;
                    if (pf != null)
                    {
                        user = pf.User;
                    }
                }
                if (user != null)
                {
                    control.Enabled = true;
                    string formName = owner.Name;
                    string controlName = control.Name;
                    PermissionCollection ps = user.GetAllPermissions(false);
                    if (ps.ExceptControl(formName, controlName))
                    {
                        control.Enabled = false;
                        return true;
                    }
                    else
                    {
                        foreach (Control c in control.Controls)
                        {
                            controlName = c.Name;
                            if (ps.ExceptControl(formName, control.Name, controlName))
                            {
                                c.Enabled = false;
                            }
                            else
                            {
                                ToolStrip ms = c as ToolStrip;
                                if (ms != null)
                                {
                                    ms.DisableForUser(user);
                                }
                                else
                                {
                                    c.DisableForUser(user);
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 用于主窗体的包含权限
        /// </summary>
        /// <param name="control"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        internal static bool EnableChildrenForUser(this Control control, User user = null)
        {
            return Enable(control, user);
        }

        private static bool Enable(Control control, User user)
        {
            Form owner = control.FindForm();
            if (owner != null)
            {
                if (user == null)
                {
                    PermissionForm pf = owner as PermissionForm;
                    if (pf != null)
                    {
                        user = pf.User;
                    }
                }
                if (user != null)
                {
                    control.Enabled = true;
                    int nofinded = 0;
                    string formName = owner.Name;
                    PermissionCollection ps = user.GetAllPermissions(false);
                    foreach (Control c in control.Controls)
                    {
                        string controlName = c.Name;
                        if (ps.ContainsControl(formName, control.Name, controlName))
                        {
                            c.Enabled = true;
                        }
                        else
                        {
                            c.Enabled = false;
                            nofinded++;
                            ToolStrip ms = c as ToolStrip;
                            if (ms != null)
                            {
                                if (ms.EnableChildrenForUser(user)) nofinded--;
                            }
                            else
                            {
                                if (c.EnableChildrenForUser(user)) nofinded--;
                            }
                        }
                    }
                    if (nofinded == control.Controls.Count) { control.Enabled = false; return false; }
                    else { return true; }
                }
            }
            return false;
        }
        /// <summary>
        /// 用于主窗体的包含权限
        /// </summary>
        /// <param name="control"></param>
        internal static void EnableAllChildren(this Control control)
        {
            List<Control> cs = new List<Control>();
            if (!control.Enabled)
            {
                EnableParent(control, ref cs);
            }
            if (cs.Count > 0)
            {
                cs.Reverse();
                foreach (Control parent in cs)
                {
                    parent.Enabled = true;
                }
            }
            control.Enabled = true;
            foreach (Control c in control.Controls)
            {
                c.Enabled = true;
            }
        }

        private static void EnableParent(Control control, ref List<Control> cs)
        {
            Control parent = control.Parent;
            if (parent != null && !parent.Enabled)
            {
                cs.Add(parent);
                EnableParent(parent, ref cs);
            }
        }
    }

    internal static class ToolStripItemExtensions
    {
        /// <summary>
        /// 用于子窗体的排除权限
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        internal static bool DisableForUser(this ToolStripItem menu, User user = null)
        {
            Form owner = null;
            if (menu.Owner != null)
            {
                if (menu.Owner.Parent != null)
                {
                    owner = menu.Owner.Parent.FindForm();
                }
            }
            if (owner != null)
            {
                if (user == null)
                {
                    PermissionForm pf = owner as PermissionForm;
                    if (pf != null)
                    {
                        user = pf.User;
                    }
                }
                if (user != null)
                {
                    menu.Enabled = true;
                    string formName = owner.Name;
                    string controlName = menu.Name;
                    PermissionCollection ps = user.GetAllPermissions(false);
                    if (ps.ExceptControl(formName, controlName))
                    {
                        menu.Enabled = false;
                        return true;
                    }
                    else
                    {
                        ToolStripDropDownItem dropmenu = menu as ToolStripDropDownItem;
                        if (dropmenu != null)
                        {
                            ToolStripItemCollection items = dropmenu.DropDownItems;
                            if (items != null)
                            {
                                foreach (ToolStripItem c in items)
                                {
                                    controlName = c.Name;
                                    if (ps.ExceptControl(formName, dropmenu.Name, controlName))
                                    {
                                        c.Enabled = false;
                                    }
                                    else
                                    {
                                        c.DisableForUser(user);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 用于子窗体的排除权限
        /// </summary>
        /// <param name="menuStrip"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        internal static bool DisableForUser(this ToolStrip menuStrip, User user = null)
        {
            return Disable(menuStrip, user);
        }

        private static bool Disable(ToolStrip menuStrip, User user)
        {
            Form owner = menuStrip.FindForm();
            if (owner != null)
            {
                if (user == null)
                {
                    PermissionForm pf = owner as PermissionForm;
                    if (pf != null)
                    {
                        user = pf.User;
                    }
                }
                if (user != null)
                {
                    menuStrip.Enabled = true;
                    string formName = owner.Name;
                    string controlName = menuStrip.Name;
                    PermissionCollection ps = user.GetAllPermissions(false);
                    if (ps.ExceptControl(formName, controlName))
                    {
                        menuStrip.Enabled = false;
                        return true;
                    }
                    else
                    {
                        foreach (ToolStripItem c in menuStrip.Items)
                        {
                            controlName = c.Name;
                            if (ps.ExceptControl(formName, menuStrip.Name, controlName))
                            {
                                c.Enabled = false;
                            }
                            else
                            {
                                c.DisableForUser(user);
                            }
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 用于主窗体的包含权限
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        internal static bool EnableChildrenForUser(this ToolStripItem menu, User user = null)
        {
            Form owner = null;
            if (menu.Owner != null)
            {
                if (menu.Owner.Parent != null)
                {
                    owner = menu.Owner.Parent.FindForm();
                }
            }
            if (owner != null)
            {
                if (user == null)
                {
                    PermissionForm pf = owner as PermissionForm;
                    if (pf != null)
                    {
                        user = pf.User;
                    }
                }
                if (user != null)
                {
                    ToolStripDropDownItem dropmenu = menu as ToolStripDropDownItem;
                    if (dropmenu != null)
                    {
                        ToolStripItemCollection items = dropmenu.DropDownItems;
                        if (items != null)
                        {
                            dropmenu.Enabled = true;
                            int nofinded = 0;
                            string formName = owner.Name;
                            PermissionCollection ps = user.GetAllPermissions(false);
                            foreach (ToolStripItem c in items)
                            {
                                string controlName = c.Name;
                                if (ps.ContainsControl(formName, dropmenu.Name, controlName))
                                {
                                    c.Enabled = true;
                                }
                                else
                                {
                                    c.Enabled = false;
                                    nofinded++;
                                    if (c.EnableChildrenForUser(user)) nofinded--;
                                }
                            }
                            if (nofinded == items.Count) { dropmenu.Enabled = false; return false; }
                            else { return true; }
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 用于主窗体的包含权限
        /// </summary>
        /// <param name="menuStrip"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        internal static bool EnableChildrenForUser(this ToolStrip menuStrip, User user = null)
        {
            return Enable(menuStrip, user);
        }

        private static bool Enable(ToolStrip menuStrip, User user)
        {
            Form owner = menuStrip.FindForm();
            if (owner != null)
            {
                if (user == null)
                {
                    PermissionForm pf = owner as PermissionForm;
                    if (pf != null)
                    {
                        user = pf.User;
                    }
                }
                if (user != null)
                {
                    menuStrip.Enabled = true;
                    int nofinded = 0;
                    string formName = owner.Name;
                    PermissionCollection ps = user.GetAllPermissions(false);
                    foreach (ToolStripItem c in menuStrip.Items)
                    {
                        string controlName = c.Name;
                        if (ps.ContainsControl(formName, menuStrip.Name, controlName))
                        {
                            c.Enabled = true;
                        }
                        else
                        {
                            c.Enabled = false;
                            nofinded++;
                            if (c.EnableChildrenForUser(user)) nofinded--;
                        }
                    }
                    if (nofinded == menuStrip.Items.Count) { menuStrip.Enabled = false; return false; }
                    else { return true; }
                }
            }
            return false;
        }
    }
}