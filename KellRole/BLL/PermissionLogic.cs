using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace KellRole
{
    public class PermissionLogic
    {
        SQLDBHelper sqlHelper;
        static PermissionLogic instance;
        public static PermissionLogic GetInstance()
        {
            if (instance == null)
                instance = new PermissionLogic();

            return instance;
        }

        private PermissionLogic()
        {
            sqlHelper = new SQLDBHelper();
        }

        public Permission GetPermission(int id)
        {
            string sql = "select * from Permission where ID=" + id;
            DataTable dt = sqlHelper.Query(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                Permission perm = new Permission();
                perm.ID = id;
                perm.Name = dt.Rows[0]["Name"].ToString();
                perm.IsExcept = Convert.ToBoolean(dt.Rows[0]["IsExcept"]);
                perm.TheModule = ModuleLogic.GetInstance().GetModule(Convert.ToInt32(dt.Rows[0]["TheModule"]));
                perm.TheAction = ActionLogic.GetInstance().GetAction(Convert.ToInt32(dt.Rows[0]["TheAction"]));
                if (dt.Rows[0]["Remark"] != null && dt.Rows[0]["Remark"] != DBNull.Value)
                    perm.Remark = dt.Rows[0]["Remark"].ToString();
                else
                    perm.Remark = "";
                return perm;
            }
            return null;
        }

        public List<Permission> GetAllPermissions()
        {
            List<Permission> perms = new List<Permission>();
            string sql = "select * from Permission";
            DataTable dt = sqlHelper.Query(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                ModuleLogic ml = ModuleLogic.GetInstance();
                ActionLogic al = ActionLogic.GetInstance();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Permission perm = new Permission();
                    perm.ID = Convert.ToInt32(dt.Rows[i]["ID"]);
                    perm.Name = dt.Rows[i]["Name"].ToString();
                    perm.IsExcept = Convert.ToBoolean(dt.Rows[i]["IsExcept"]);
                    perm.TheModule = ml.GetModule(Convert.ToInt32(dt.Rows[i]["TheModule"]));
                    perm.TheAction = al.GetAction(Convert.ToInt32(dt.Rows[i]["TheAction"]));
                    if (dt.Rows[i]["Remark"] != null && dt.Rows[i]["Remark"] != DBNull.Value)
                        perm.Remark = dt.Rows[i]["Remark"].ToString();
                    else
                        perm.Remark = "";
                    perms.Add(perm);
                }
            }
            return perms;
        }

        public int AddPermission(Permission perm)
        {
            string sql = "";
            if (perm.TheAction != null)
                sql = "insert into Permission (Name, IsExcept, TheModule, TheAction, Remark) values ('" + perm.Name + "'," + (perm.IsExcept ? "1" : "0") + ", " + perm.TheModule.ID + ", " + perm.TheAction.ID + ", '" + perm.Remark + "'); select SCOPE_IDENTITY()";
            else
                sql = "insert into Permission (Name, IsExcept, TheModule, Remark) values ('" + perm.Name + "', " + (perm.IsExcept ? "1" : "0") + ", " + perm.TheModule.ID + ", '" + perm.Remark + "'); select SCOPE_IDENTITY()";
            object obj = sqlHelper.ExecuteSqlReturn(sql);
            int R;
            if (obj != null && obj != DBNull.Value && int.TryParse(obj.ToString(), out R))
                return R;
            else
                return 0;
        }

        public bool UpdatePermission(Permission perm)
        {
            string sql = "";
            if (perm.TheAction != null)
                sql = "update Permission set Name='" + perm.Name + "', IsExcept=" + (perm.IsExcept ? "1" : "0") + ", TheModule=" + perm.TheModule.ID + ", TheAction=" + perm.TheAction.ID + ", Remark='" + perm.Remark + "' where ID=" + perm.ID;
            else
                sql = "update Permission set Name='" + perm.Name + "', IsExcept=" + (perm.IsExcept ? "1" : "0") + ", TheModule=" + perm.TheModule.ID + ", TheAction=0, Remark='" + perm.Remark + "' where ID=" + perm.ID;
            int r = sqlHelper.ExecuteSql(sql);
            return r > 0;
        }

        public bool DeletePermission(Permission perm)
        {
            string sql = "delete from Permission where ID=" + perm.ID;
            int r = sqlHelper.ExecuteSql(sql);
            return r > 0;
        }
        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool UpgradeList(List<Permission> list)
        {
            int errCount = 0;
            foreach (Permission perm in list)
            {
                string sqlStr = "";
                if (perm.TheAction != null)
                    sqlStr = "if exists (select 1 from Permission where ID=" + perm.ID + ") update Permission set Name='" + perm.Name + "', IsExcept=" + (perm.IsExcept ? "1" : "0") + ", TheModule=" + perm.TheModule.ID + ", TheAction=" + perm.TheAction.ID + ", Remark='" + perm.Remark + "' where ID=" + perm.ID + " else insert into Permission (Name, IsExcept, TheModule, TheAction, Remark) values ('" + perm.Name + "', " + (perm.IsExcept ? "1" : "0") + ", " + perm.TheModule.ID + ", " + perm.TheAction.ID + ", '" + perm.Remark + "')";
                else
                    sqlStr = "if exists (select 1 from Permission where ID=" + perm.ID + ") update Permission set Name='" + perm.Name + "', IsExcept=" + (perm.IsExcept ? "1" : "0") + ", TheModule=" + perm.TheModule.ID + ", TheAction=0, Remark='" + perm.Remark + "' where ID=" + perm.ID + " else insert into Permission (Name, IsExcept, TheModule, Remark) values ('" + perm.Name + "', " + (perm.IsExcept ? "1" : "0") + ", " + perm.TheModule.ID + ", '" + perm.Remark + "')";
                try
                {
                    sqlHelper.ExecuteSql(sqlStr);
                }
                catch (Exception)
                {
                    errCount++;
                }
            }
            return errCount == 0;
        }

        /// <summary>
        /// 是否存在同名
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool ExistsName(string name)
        {
            return sqlHelper.Exists("select 1 from Permission where Name='" + name + "'");
        }

        /// <summary>
        /// 是否存在出了自己以外的同名
        /// </summary>
        /// <param name="name"></param>
        /// <param name="myId"></param>
        /// <returns></returns>
        public bool ExistsNameOther(string name, int myId)
        {
            return sqlHelper.Exists("select 1 from Permission where ID!=" + myId + " and Name='" + name + "'");
        }

        /// <summary>
        /// 是否存在指定条件的记录
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool ExistsWhere(string where)
        {
            if (!string.IsNullOrEmpty(where))
            {
                string w = where.Trim().ToLower();
                if (!w.StartsWith("where "))
                    w = "where " + w;
                return sqlHelper.Exists("select 1 from Permission " + w);
            }
            return false;
        }
    }
}
