using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace KellRole
{
    public class ModuleLogic
    {
        SQLDBHelper sqlHelper;
        static ModuleLogic instance;
        public static ModuleLogic GetInstance()
        {
            if (instance == null)
                instance = new ModuleLogic();

            return instance;
        }

        private ModuleLogic()
        {
            sqlHelper = new SQLDBHelper();
        }

        public Module GetModule(int id)
        {
            string sql = "select * from Module where ID=" + id;
            DataTable dt = sqlHelper.Query(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                Module module = new Module();
                module.ID = id;
                module.Name = dt.Rows[0]["Name"].ToString();
                if (dt.Rows[0]["FormName"] != null && dt.Rows[0]["FormName"] != DBNull.Value)
                    module.FormName = dt.Rows[0]["FormName"].ToString();
                else
                    module.FormName = "";
                if (dt.Rows[0]["ControlName"] != null && dt.Rows[0]["ControlName"] != DBNull.Value)
                    module.ControlName = dt.Rows[0]["ControlName"].ToString();
                else
                    module.ControlName = "";
                if (dt.Rows[0]["Remark"] != null && dt.Rows[0]["Remark"] != DBNull.Value)
                    module.Remark = dt.Rows[0]["Remark"].ToString();
                else
                    module.Remark = "";
                return module;
            }
            return null;
        }

        public List<Module> GetAllModules()
        {
            List<Module> modules = new List<Module>();
            string sql = "select * from Module";
            DataTable dt = sqlHelper.Query(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Module module = new Module();
                    module.ID = Convert.ToInt32(dt.Rows[i]["ID"]);
                    module.Name = dt.Rows[i]["Name"].ToString();
                    if (dt.Rows[i]["FormName"] != null && dt.Rows[i]["FormName"] != DBNull.Value)
                        module.FormName = dt.Rows[i]["FormName"].ToString();
                    else
                        module.FormName = "";
                    if (dt.Rows[i]["ControlName"] != null && dt.Rows[i]["ControlName"] != DBNull.Value)
                        module.ControlName = dt.Rows[i]["ControlName"].ToString();
                    else
                        module.ControlName = "";
                    if (dt.Rows[i]["Remark"] != null && dt.Rows[i]["Remark"] != DBNull.Value)
                        module.Remark = dt.Rows[i]["Remark"].ToString();
                    else
                        module.Remark = "";
                    modules.Add(module);
                }
            }
            return modules;
        }

        public int AddModule(Module module)
        {
            string sql = "insert into Module (Name, FormName, ControlName, Remark) values ('" + module.Name + "', '" + module.FormName + "', '" + module.ControlName + "', '" + module.Remark + "'); select SCOPE_IDENTITY()";
            object obj = sqlHelper.ExecuteSqlReturn(sql);
            int R;
            if (obj != null && obj != DBNull.Value && int.TryParse(obj.ToString(), out R))
                return R;
            else
                return 0;
        }

        public bool UpdateModule(Module module)
        {
            string sql = "update Module set Name='" + module.Name + "', FormName='" + module.FormName + "', ControlName='" + module.ControlName + "', Remark='" + module.Remark + "' where ID=" + module.ID;
            int r = sqlHelper.ExecuteSql(sql);
            return r > 0;
        }

        public bool DeleteModule(Module module)
        {
            string sql = "delete from Module where ID=" + module.ID;
            int r = sqlHelper.ExecuteSql(sql);
            return r > 0;
        }
        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool UpgradeList(List<Module> list)
        {
            int errCount = 0;
            foreach (Module module in list)
            {
                string sqlStr = "if exists (select 1 from Module where ID=" + module.ID + ") update Module set Name='" + module.Name + "', FormName='" + module.FormName + "', ControlName='" + module.ControlName + "', Remark='" + module.Remark + "' where ID=" + module.ID + " else insert into Module (Name, FormName, ControlName, Remark) values ('" + module.Name + "', '" + module.FormName + "', '" + module.ControlName + "', '" + module.Remark + "')";
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
            return sqlHelper.Exists("select 1 from Module where Name='" + name + "'");
        }

        /// <summary>
        /// 是否存在出了自己以外的同名
        /// </summary>
        /// <param name="name"></param>
        /// <param name="myId"></param>
        /// <returns></returns>
        public bool ExistsNameOther(string name, int myId)
        {
            return sqlHelper.Exists("select 1 from Module where ID!=" + myId + " and Name='" + name + "'");
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
                return sqlHelper.Exists("select 1 from Module " + w);
            }
            return false;
        }
    }
}
