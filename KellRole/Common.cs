using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Net;
using System.ComponentModel;
using System.Reflection;
using System.Configuration;

namespace KellRole
{
    public static class Common
    {
        private static int adminId;

        public static bool SaveConnectionString(string name, SqlConnectionStringBuilder scsb, string configPath = null)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + AppDomain.CurrentDomain.FriendlyName + ".config";
            if (!string.IsNullOrEmpty(configPath))
                path = configPath;
            if (!File.Exists(path))
                return false;
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(path);
            XmlNode xNode;
            XmlElement xElem;
            xNode = xDoc.SelectSingleNode("//connectionStrings");
            if (xNode != null)
            {
                xElem = (XmlElement)xNode.SelectSingleNode("//add[@name='" + name + "']");
                if (xElem != null)
                {
                    xElem.SetAttribute("connectionString", string.Format("Data Source={0};User ID={1};Password={2};Initial Catalog={3}", scsb.DataSource, scsb.UserID, scsb.Password, scsb.InitialCatalog));
                    xDoc.Save(path);
                    return true;
                }
            }
            return false;
        }

        public static string GetConnectionString(string name, string configPath = null)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + AppDomain.CurrentDomain.FriendlyName + ".config";
            if (!string.IsNullOrEmpty(configPath))
                path = configPath;
            if (!File.Exists(path))
                return "";
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(path);
            XmlNode xNode;
            XmlElement xElem;
            xNode = xDoc.SelectSingleNode("//connectionStrings");
            if (xNode != null)
            {
                xElem = (XmlElement)xNode.SelectSingleNode("//add[@name='" + name + "']");
                if (xElem != null)
                {
                    string s = xElem.GetAttribute("connectionString");
                    return s;
                }
            }
            return "";
        }

        public static string ConnString
        {
            get
            {
                //string connString = string.Empty;
                //ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["connString"];
                //if (settings != null)
                //{
                //    connString = settings.ConnectionString;
                //}
                //return connString;
                return GetConnectionString("connString");
            }
        }

        public static string BackupConnString
        {
            get
            {
                //string connString = string.Empty;
                //ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["backupConnString"];
                //if (settings != null)
                //{
                //    connString = settings.ConnectionString;
                //}
                //return connString;
                return GetConnectionString("backupConnString");
            }
        }

        public static RoleCollection GetRoles(string roleids, RoleLogic rl = null)
        {
            RoleCollection roles = new RoleCollection();
            string[] ids = roleids.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (rl == null) rl = RoleLogic.GetInstance();
            foreach (string id in ids)
            {
                int I;
                if (int.TryParse(id, out I))
                {
                    Role role = rl.GetRole(I);
                    roles.Add(role);
                }
            }
            return roles;
        }

        public static List<UserGroup> GetUserGroups(string ugroups, UserGroupLogic ul = null)
        {
            List<UserGroup> ugrps = new List<UserGroup>();
            string[] ids = ugroups.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (ul == null) ul = UserGroupLogic.GetInstance();
            foreach (string id in ids)
            {
                int I;
                if (int.TryParse(id, out I))
                {
                    UserGroup ug = ul.GetUserGroup(I);
                    ugrps.Add(ug);
                }
            }
            return ugrps;
        }

        public static List<Department> GetDepartments(string deps, DepartmentLogic dl = null)
        {
            List<Department> depts = new List<Department>();
            string[] ids = deps.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (dl == null) dl = DepartmentLogic.GetInstance();
            foreach (string id in ids)
            {
                int I;
                if (int.TryParse(id, out I))
                {
                    Department dep = dl.GetDepartment(I);
                    depts.Add(dep);
                }
            }
            return depts;
        }

        public static List<int> GetRoleIds(string roleids, RoleLogic rl = null)
        {
            List<int> roles = new List<int>();
            string[] ids = roleids.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (rl == null) rl = RoleLogic.GetInstance();
            foreach (string id in ids)
            {
                int I;
                if (int.TryParse(id, out I))
                {
                    roles.Add(I);
                }
            }
            return roles;
        }

        public static List<int> GetUserGroupIds(string ugroups, UserGroupLogic ul = null)
        {
            List<int> ugrps = new List<int>();
            string[] ids = ugroups.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (ul == null) ul = UserGroupLogic.GetInstance();
            foreach (string id in ids)
            {
                int I;
                if (int.TryParse(id, out I))
                {
                    ugrps.Add(I);
                }
            }
            return ugrps;
        }

        public static List<int> GetDepartmentIds(string deps, DepartmentLogic dl = null)
        {
            List<int> depts = new List<int>();
            string[] ids = deps.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (dl == null) dl = DepartmentLogic.GetInstance();
            foreach (string id in ids)
            {
                int I;
                if (int.TryParse(id, out I))
                {
                    depts.Add(I);
                }
            }
            return depts;
        }

        public static PermissionCollection GetPermissions(string pers, PermissionLogic pl = null)
        {
            PermissionCollection perms = new PermissionCollection();
            string[] ids = pers.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (pl == null) pl = PermissionLogic.GetInstance();
            foreach (string id in ids)
            {
                int I;
                if (int.TryParse(id, out I))
                {
                    Permission perm = pl.GetPermission(I);
                    perms.Add(perm);
                }
            }
            return perms;
        }

        public static List<int> GetPermissionIds(string pers, PermissionLogic pl = null)
        {
            List<int> perms = new List<int>();
            string[] ids = pers.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (pl == null) pl = PermissionLogic.GetInstance();
            foreach (string id in ids)
            {
                int I;
                if (int.TryParse(id, out I))
                {
                    perms.Add(I);
                }
            }
            return perms;
        }

        public static string GetRolesStr(RoleCollection roles)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Role role in roles)
            {
                if (sb.Length == 0)
                    sb.Append(role.ID.ToString());
                else
                    sb.Append("," + role.ID.ToString());
            }
            return sb.ToString();
        }

        public static string GetPermissionsStr(PermissionCollection perms)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Permission perm in perms)
            {
                if (sb.Length == 0)
                    sb.Append(perm.ID.ToString());
                else
                    sb.Append("," + perm.ID.ToString());
            }
            return sb.ToString();
        }

        public static string GetUserGroupsStr(List<UserGroup> ugrps)
        {
            StringBuilder sb = new StringBuilder();
            foreach (UserGroup ug in ugrps)
            {
                if (sb.Length == 0)
                    sb.Append(ug.ID.ToString());
                else
                    sb.Append("," + ug.ID.ToString());
            }
            return sb.ToString();
        }

        public static string GetDepartmentsStr(List<Department> deps)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Department dep in deps)
            {
                if (sb.Length == 0)
                    sb.Append(dep.ID.ToString());
                else
                    sb.Append("," + dep.ID.ToString());
            }
            return sb.ToString();
        }

        public static string GetRolesStr(List<int> roles)
        {
            StringBuilder sb = new StringBuilder();
            foreach (int role in roles)
            {
                if (sb.Length == 0)
                    sb.Append(role.ToString());
                else
                    sb.Append("," + role.ToString());
            }
            return sb.ToString();
        }

        public static string GetPermissionsStr(List<int> perms)
        {
            StringBuilder sb = new StringBuilder();
            foreach (int perm in perms)
            {
                if (sb.Length == 0)
                    sb.Append(perm.ToString());
                else
                    sb.Append("," + perm.ToString());
            }
            return sb.ToString();
        }

        public static string GetUserGroupsStr(List<int> ugrps)
        {
            StringBuilder sb = new StringBuilder();
            foreach (int ug in ugrps)
            {
                if (sb.Length == 0)
                    sb.Append(ug.ToString());
                else
                    sb.Append("," + ug.ToString());
            }
            return sb.ToString();
        }

        public static string GetDepartmentsStr(List<int> deps)
        {
            StringBuilder sb = new StringBuilder();
            foreach (int dep in deps)
            {
                if (sb.Length == 0)
                    sb.Append(dep.ToString());
                else
                    sb.Append("," + dep.ToString());
            }
            return sb.ToString();
        }

        public static int AdminId
        {
            get
            {
                if (adminId == 0)
                {
                    int id = 0;
                    User admin = UserLogic.GetInstance().GetUser(CommonConsts.AdminName);
                    if (admin != null)
                        id = admin.ID;
                    adminId = id;
                }
                return adminId;
            }
        }

        /// <summary>
        /// 获取指定类型的描述（用于提取订单状态的枚举名字说明）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public static string GetDescription<T>(string propertyName, bool ignoreCase = true)
        {
            string desc = string.Empty;
            PropertyInfo[] peroperties = typeof(T).GetProperties(BindingFlags.Default);
            foreach (PropertyInfo property in peroperties)
            {
                if (ignoreCase)
                {
                    if (property.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        object[] objs = property.GetCustomAttributes(typeof(DescriptionAttribute), true);
                        if (objs.Length > 0)
                        {
                            desc = ((DescriptionAttribute)objs[0]).Description;
                        }
                        break;
                    }
                }
                else
                {
                    if (property.Name.Equals(propertyName, StringComparison.InvariantCulture))
                    {
                        object[] objs = property.GetCustomAttributes(typeof(DescriptionAttribute), true);
                        if (objs.Length > 0)
                        {
                            desc = ((DescriptionAttribute)objs[0]).Description;
                        }
                        break;
                    }
                }
            }
            return desc;
        }

        /// <summary>
        /// 获取指定主机的IPv4地址
        /// </summary>
        /// <param name="hostNameOrAddress"></param>
        /// <returns></returns>
        public static List<IPAddress> GetIP4(string hostNameOrAddress)
        {
            List<IPAddress> addresses = new List<IPAddress>();
            IPAddress[] ips = Dns.GetHostAddresses(hostNameOrAddress);
            foreach (IPAddress ip in ips)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    addresses.Add(ip);
            }
            return addresses;
        }

        /// <summary>
        /// 获取指定主机的IPv6地址
        /// </summary>
        /// <param name="hostNameOrAddress"></param>
        /// <returns></returns>
        public static List<IPAddress> GetIP6(string hostNameOrAddress)
        {
            List<IPAddress> addresses = new List<IPAddress>();
            IPAddress[] ips = Dns.GetHostAddresses(hostNameOrAddress);
            foreach (IPAddress ip in ips)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                    addresses.Add(ip);
            }
            return addresses;
        }

        /// <summary>
        /// 获取MD5密文的小写字符串
        /// </summary>
        /// <param name="orgin"></param>
        /// <returns></returns>
        public static string GetMD5(string orgin)
        {
            StringBuilder sb = new StringBuilder();
            using (MD5 md5 = MD5.Create())
            {
                byte[] buffer = Encoding.UTF8.GetBytes(orgin);
                byte[] hash = md5.ComputeHash(buffer);
                foreach (byte i in hash)
                {
                    sb.Append(i.ToString("x2"));
                }
            }
            return sb.ToString();
        }

        /// <summary>  
        /// 时间戳转为DateTime时间格式  
        /// </summary>  
        /// <param name="timeStamp">时间戳格式</param>  
        /// <returns>DateTime时间格式</returns>  
        public static DateTime TimeStampToDateTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }

        /// <summary>  
        /// DateTime时间格式转换为时间戳格式  
        /// </summary>  
        /// <param name="time"> DateTime时间格式</param>  
        /// <returns>时间戳格式</returns>  
        public static string DateTimeToTimeStamp(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            long lTime = (long)(time - startTime).TotalMilliseconds;
            return lTime.ToString();
        }
    }

    public static class CommonConsts
    {
        public static string DefaultConnString
        {
            get
            {
                return @"server=.;database=Role;user id=sa;password=damaodf";
            }
        }

        public static string DefaultBackupConnString
        {
            get
            {
                return @"server=.;database=backup;user id=sa;password=damaodf";
            }
        }

        public static string ConnString
        {
            get
            {
                //string connString = string.Empty;
                //ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["connString"];
                //if (settings != null)
                //{
                //    connString = settings.ConnectionString;
                //}
                //return connString;
                return Common.GetConnectionString("connString");
            }
        }

        public static string BackupConnString
        {
            get
            {
                //string connString = string.Empty;
                //ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["backupConnString"];
                //if (settings != null)
                //{
                //    connString = settings.ConnectionString;
                //}
                //return connString;
                return Common.GetConnectionString("backupConnString");
            }
        }

        public static string Auth
        {
            get
            {
                string val = string.Empty;
                string raw = ConfigurationManager.AppSettings["auth"];
                if (!string.IsNullOrEmpty(raw))
                    val = raw;
                return val;
            }
        }

        public static string AdminName
        {
            get
            {
                string val = "kell";
                string raw = ConfigurationManager.AppSettings["AdminName"];
                if (!string.IsNullOrEmpty(raw))
                    val = raw;
                return val;
            }
        }
    }
}
