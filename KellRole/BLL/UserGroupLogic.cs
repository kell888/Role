﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace KellRole
{
    public class UserGroupLogic
    {
        SQLDBHelper sqlHelper;
        static UserGroupLogic instance;
        public static UserGroupLogic GetInstance()
        {
            if (instance == null)
                instance = new UserGroupLogic();

            return instance;
        }

        private UserGroupLogic()
        {
            sqlHelper = new SQLDBHelper();
        }

        public UserGroup GetUserGroup(int id)
        {
            string sql = "select * from UserGroup where ID=" + id;
            DataTable dt = sqlHelper.Query(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                UserGroup ug = new UserGroup();
                ug.ID = id;
                ug.Name = dt.Rows[0]["Name"].ToString();
                if (dt.Rows[0]["Remark"] != null && dt.Rows[0]["Remark"] != DBNull.Value)
                    ug.Remark = dt.Rows[0]["Remark"].ToString();
                else
                    ug.Remark = "";
                return ug;
            }
            return null;
        }

        public List<UserGroup> GetAllUserGroups()
        {
            List<UserGroup> ugs = new List<UserGroup>();
            string sql = "select * from UserGroup";
            DataTable dt = sqlHelper.Query(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    UserGroup ug = new UserGroup();
                    ug.ID = Convert.ToInt32(dt.Rows[i]["ID"]);
                    ug.Name = dt.Rows[i]["Name"].ToString();
                    if (dt.Rows[i]["Remark"] != null && dt.Rows[i]["Remark"] != DBNull.Value)
                        ug.Remark = dt.Rows[i]["Remark"].ToString();
                    else
                        ug.Remark = "";
                    ugs.Add(ug);
                }
            }
            return ugs;
        }

        public int AddUserGroup(UserGroup ug)
        {
            string sql = "insert into UserGroup (Name, Remark) values ('" + ug.Name + "', '" + ug.Remark + "'); select SCOPE_IDENTITY()";
            object obj = sqlHelper.ExecuteSqlReturn(sql);
            int R;
            if (obj != null && obj != DBNull.Value && int.TryParse(obj.ToString(), out R))
                return R;
            else
                return 0;
        }

        public bool UpdateUserGroup(UserGroup ug)
        {
            string sql = "update UserGroup set Name='" + ug.Name + "', Remark='" + ug.Remark + "' where ID=" + ug.ID;
            int r = sqlHelper.ExecuteSql(sql);
            return r > 0;
        }

        public bool DeleteUserGroup(UserGroup ug)
        {
            string sql = "delete from UserGroup where ID=" + ug.ID;
            int r = sqlHelper.ExecuteSql(sql);
            return r > 0;
        }
        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool UpgradeList(List<UserGroup> list)
        {
            int errCount = 0;
            foreach (UserGroup ug in list)
            {
                string sqlStr = "if exists (select 1 from Module where ID=" + ug.ID + ") update UserGroup set Name='" + ug.Name + "', Roles='" + Common.GetRolesStr(ug.Roles) + "', Remark='" + ug.Remark + "' where ID=" + ug.ID + " else insert into UserGroup (Name, Roles, Remark) values ('" + ug.Name + "', '" + Common.GetRolesStr(ug.Roles) + "', '" + ug.Remark + "')";
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
            return sqlHelper.Exists("select 1 from UserGroup where Name='" + name + "'");
        }

        /// <summary>
        /// 是否存在出了自己以外的同名
        /// </summary>
        /// <param name="name"></param>
        /// <param name="myId"></param>
        /// <returns></returns>
        public bool ExistsNameOther(string name, int myId)
        {
            return sqlHelper.Exists("select 1 from UserGroup where ID!=" + myId + " and Name='" + name + "'");
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
                return sqlHelper.Exists("select 1 from UserGroup " + w);
            }
            return false;
        }
    }

    public static class UserGroupExtensions
    {
        public static RoleCollection Roles(this UserGroup thisUG)
        {
            RoleCollection rs = Common.GetRoles(Common.GetRolesStr(thisUG.Roles));
            return rs;
        }
    }
}
