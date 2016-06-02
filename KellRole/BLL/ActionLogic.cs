﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace KellRole
{
    public class ActionLogic
    {
        SQLDBHelper sqlHelper;
        static ActionLogic instance;
        public static ActionLogic GetInstance()
        {
            if (instance == null)
                instance = new ActionLogic();

            return instance;
        }

        private ActionLogic()
        {
            sqlHelper = new SQLDBHelper();
        }

        public Action GetAction(int id)
        {
            string sql = "select * from [Action] where ID="+id;
            DataTable dt = sqlHelper.Query(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                Action action = new Action();
                action.ID = id;
                action.Name = dt.Rows[0]["Name"].ToString();
                if (dt.Rows[0]["FormName"] != null && dt.Rows[0]["FormName"] != DBNull.Value)
                    action.FormName = dt.Rows[0]["FormName"].ToString();
                else
                    action.FormName = "";
                if (dt.Rows[0]["ControlName"] != null && dt.Rows[0]["ControlName"] != DBNull.Value)
                    action.ControlName = dt.Rows[0]["ControlName"].ToString();
                else
                    action.ControlName = "";
                if (dt.Rows[0]["Remark"] != null && dt.Rows[0]["Remark"] != DBNull.Value)
                    action.Remark = dt.Rows[0]["Remark"].ToString();
                else
                    action.Remark = "";
                return action;
            }
            return null;
        }

        public List<Action> GetAllActions()
        {
            List<Action> actions = new List<Action>();
            string sql = "select * from [Action]";
            DataTable dt = sqlHelper.Query(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Action action = new Action();
                    action.ID = Convert.ToInt32(dt.Rows[i]["ID"]);
                    action.Name = dt.Rows[i]["Name"].ToString();
                    if (dt.Rows[i]["FormName"] != null && dt.Rows[i]["FormName"] != DBNull.Value)
                        action.FormName = dt.Rows[i]["FormName"].ToString();
                    else
                        action.FormName = "";
                    if (dt.Rows[i]["ControlName"] != null && dt.Rows[i]["ControlName"] != DBNull.Value)
                        action.ControlName = dt.Rows[i]["ControlName"].ToString();
                    else
                        action.ControlName = "";
                    if (dt.Rows[i]["Remark"] != null && dt.Rows[i]["Remark"] != DBNull.Value)
                        action.Remark = dt.Rows[i]["Remark"].ToString();
                    else
                        action.Remark = "";
                    actions.Add(action);
                }
            }
            return actions;
        }

        public int AddAction(Action action)
        {
            string sql = "insert into [Action] (Name, FormName, ControlName, Remark) values ('" + action.Name + "', '"+action.FormName+"', '"+action.ControlName+"', '" + action.Remark + "'); select SCOPE_IDENTITY()";
            object obj = sqlHelper.ExecuteSqlReturn(sql);
            int R;
            if (obj != null && obj != DBNull.Value && int.TryParse(obj.ToString(), out R))
                return R;
            else
                return 0;
        }

        public bool UpdateAction(Action action)
        {
            string sql = "update [Action] set Name='" + action.Name + "', FormName='" + action.FormName + "', ControlName='" + action.ControlName + "', Remark='" + action.Remark + "' where ID=" + action.ID;
            int r = sqlHelper.ExecuteSql(sql);
            return r > 0;
        }

        public bool DeleteAction(Action action)
        {
            string sql = "delete from [Action] where ID=" + action.ID;
            int r = sqlHelper.ExecuteSql(sql);
            return r > 0;
        }
        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool UpgradeList(List<Action> list)
        {
            int errCount = 0;
            foreach (Action action in list)
            {
                string sqlStr = "if exists (select 1 from [Action] where ID=" + action.ID + ") update [Action] set Name='" + action.Name + "', FormName='" + action.FormName + "', ControlName='" + action.ControlName + "', Remark='" + action.Remark + "' where ID=" + action.ID + " else insert into [Action] (Name, FormName, ControlName, Remark) values ('" + action.Name + "', '" + action.FormName + "', '" + action.ControlName + "', '" + action.Remark + "')";
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
            return sqlHelper.Exists("select 1 from [Action] where Name='" + name + "'");
        }

        /// <summary>
        /// 是否存在出了自己以外的同名
        /// </summary>
        /// <param name="name"></param>
        /// <param name="myId"></param>
        /// <returns></returns>
        public bool ExistsNameOther(string name, int myId)
        {
            return sqlHelper.Exists("select 1 from [Action] where ID!=" + myId + " and Name='" + name + "'");
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
                return sqlHelper.Exists("select 1 from [Action] " + w);
            }
            return false;
        }
    }
}
