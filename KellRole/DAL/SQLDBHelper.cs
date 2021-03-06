﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Collections;
using System.Windows.Forms;
using System.Threading;

namespace KellRole
{
    public class SQLDBHelper
    {
        private static readonly object lockObj = new object();
        #region 公用方法
         
        /// <summary>
        /// 判断是否存在某表的某个字段
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <param name="columnName">列名称</param>
        /// <returns>是否存在</returns>
        public bool ColumnExists(string tableName, string columnName, bool backupDB = false)
        {
            lock (lockObj)
            {
                string sql = "select count(1) from syscolumns where [id]=object_id('" + tableName + "') and [name]='" + columnName + "'";
                object res = GetSingle(sql, backupDB);
                if (res == null)
                {
                    return false;
                }
                return Convert.ToInt32(res) > 0;
            }
        }
        //获得最大列数
        public int GetMaxID(string FieldName, string TableName, bool backupDB = false)
        {
            lock (lockObj)
            {
                string strsql = "select max(" + FieldName + ") from " + TableName;
                object obj = GetSingle(strsql, backupDB);
                if (obj == null)
                {
                    return 1;
                }
                else
                {
                    return int.Parse(obj.ToString());
                }
            }
        }
        //获得最小列数
        public int GetMinId(string FieldName, string TableName, bool backupDB = false)
        {
            lock (lockObj)
            {
                string strsql = "select min(" + FieldName + ") from " + TableName;
                object obj = GetSingle(strsql, backupDB);
                if (obj == null)
                {
                    return 1;
                }
                else
                {
                    return int.Parse(obj.ToString());
                }
            }
        }
        //获得该列的最大/小值
        public string GetMaxOrMinIndex(string columnName, string tableName, string orderBy, bool backupDB = false)
        {
            lock (lockObj)
            {
                string strsql = "select top 1 case when " + columnName + " is null then 0 else " + columnName + " end as Index from " + tableName + " order by " + tableName + orderBy;
                object obj = GetSingle(strsql, backupDB);
                if (obj == null)
                {
                    return "";
                }
                else
                {
                    return obj.ToString();
                }
            }
        }

        /// <summary>
        /// 判断表是否存在的方法
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public bool Exists(string strSql, bool backupDB = false)
        {
            lock (lockObj)
            {
                object obj = GetSingle(strSql, backupDB);
                int cmdresult;
                if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                {
                    cmdresult = 0;
                }
                else
                {
                    cmdresult = int.Parse(obj.ToString());
                }
                if (cmdresult == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        /// <summary>
        /// 表是否存在
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public bool TabExists(string TableName, bool backupDB = false)
        {
            lock (lockObj)
            {
                string strsql = "select count(*) from sysobjects where id = object_id(N'[" + TableName + "]') and OBJECTPROPERTY(id, N'IsUserTable') = 1";
                //string strsql = "SELECT count(*) FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[" + TableName + "]') AND type in (N'U')";
                object obj = GetSingle(strsql, backupDB);
                int cmdresult;
                if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                {
                    cmdresult = 0;
                }
                else
                {
                    cmdresult = int.Parse(obj.ToString());
                }
                if (cmdresult == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        public bool Exists(string strSql, bool backupDB = false, params SqlParameter[] cmdParms)
        {
            lock (lockObj)
            {
                object obj = GetSingle(strSql, backupDB, cmdParms);
                int cmdresult;
                if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                {
                    cmdresult = 0;
                }
                else
                {
                    cmdresult = int.Parse(obj.ToString());
                }
                if (cmdresult == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        #endregion

        #region  执行简单SQL语句

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public int ExecuteSql(string SQLString, bool backupDB = false)
        {
            lock (lockObj)
            {
                using (SqlConnection connection = HelpConnection.GetConnection(backupDB))
                {
                    using (SqlCommand cmd = new SqlCommand(SQLString, connection))
                    {
                        try
                        {
                            connection.Open();
                            int rows = cmd.ExecuteNonQuery();
                            return rows;
                        }
                        catch (System.Data.SqlClient.SqlException e)
                        {
                            //WriteLog.CreateLog("数据库操作日志", "DbHelperSQL类（ExecuteSql函数）", "error", e.Message);
                            MessageBox.Show("数据库操作失败：" + e.Message);
                            return -1;
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
                }
            }
        }

        public int ExecuteSqlByTime(string SQLString, int Times, bool backupDB = false)
        {
            lock (lockObj)
            {
                using (SqlConnection connection = HelpConnection.GetConnection(backupDB))
                {
                    using (SqlCommand cmd = new SqlCommand(SQLString, connection))
                    {
                        try
                        {
                            connection.Open();
                            cmd.CommandTimeout = Times;
                            int rows = cmd.ExecuteNonQuery();
                            return rows;
                        }
                        catch (System.Data.SqlClient.SqlException e)
                        {
                            connection.Close();
                            //WriteLog.CreateLog("数据库操作日志", "DbHelperSQL类（ExecuteSqlByTime函数）:", "error", e.Message);
                            MessageBox.Show("数据库操作失败：" + e.Message);
                            return -1;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 执行Sql和Oracle滴混合事务
        /// </summary>
        /// <param name="list">SQL命令行列表</param>
        /// <param name="oracleCmdSqlList">Oracle命令行列表</param>
        /// <returns>执行结果 0-由于SQL造成事务失败 -1 由于Oracle造成事务失败 1-整体事务执行成功</returns>
        public int ExecuteSqlTran(List<CommandInfo> list, List<CommandInfo> oracleCmdSqlList, bool backupDB = false)
        {
            lock (lockObj)
            {
                using (SqlConnection conn = HelpConnection.GetConnection(backupDB))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    SqlTransaction tx = conn.BeginTransaction();
                    cmd.Transaction = tx;
                    try
                    {
                        foreach (CommandInfo myDE in list)
                        {
                            string cmdText = myDE.CommandText;
                            SqlParameter[] cmdParms = (SqlParameter[])myDE.Parameters;
                            PrepareCommand(cmd, conn, tx, cmdText, cmdParms);
                            if (myDE.EffentNextType == EffentNextType.SolicitationEvent)
                            {
                                if (myDE.CommandText.ToLower().IndexOf("count(") == -1)
                                {
                                    tx.Rollback();
                                    MessageBox.Show("数据库操作失败：违背要求" + myDE.CommandText + "必须符合select count(..的格式");
                                    return -1;
                                }

                                object obj = cmd.ExecuteScalar();
                                bool isHave = false;
                                if (obj == null && obj == DBNull.Value)
                                {
                                    isHave = false;
                                }
                                isHave = Convert.ToInt32(obj) > 0;
                                if (isHave)
                                {
                                    //引发事件
                                    myDE.OnSolicitationEvent();
                                }
                            }
                            if (myDE.EffentNextType == EffentNextType.WhenHaveContine || myDE.EffentNextType == EffentNextType.WhenNoHaveContine)
                            {
                                if (myDE.CommandText.ToLower().IndexOf("count(") == -1)
                                {
                                    tx.Rollback();
                                    MessageBox.Show("数据库操作失败：违背要求" + myDE.CommandText + "必须符合select count(..的格式");
                                    return -1;
                                }

                                object obj = cmd.ExecuteScalar();
                                bool isHave = false;
                                if (obj == null && obj == DBNull.Value)
                                {
                                    isHave = false;
                                }
                                isHave = Convert.ToInt32(obj) > 0;

                                if (myDE.EffentNextType == EffentNextType.WhenHaveContine && !isHave)
                                {
                                    tx.Rollback();
                                    MessageBox.Show("数据库操作失败：违背要求" + myDE.CommandText + "返回值必须大于0");
                                    return -1;
                                }
                                if (myDE.EffentNextType == EffentNextType.WhenNoHaveContine && isHave)
                                {
                                    tx.Rollback();
                                    MessageBox.Show("数据库操作失败：违背要求" + myDE.CommandText + "返回值必须等于0");
                                    return -1;
                                }
                                continue;
                            }
                            int val = cmd.ExecuteNonQuery();
                            if (myDE.EffentNextType == EffentNextType.ExcuteEffectRows && val == 0)
                            {
                                tx.Rollback();
                                MessageBox.Show("数据库操作失败：违背要求" + myDE.CommandText + "必须有影响行");
                                return -1;
                            }
                            cmd.Parameters.Clear();
                        }
                        return 1;
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        tx.Rollback();
                        //WriteLog.CreateLog("数据库操作日志", "DbHelperSQL类（ExecuteSqlTran函数）:", "error", e.Message);
                        MessageBox.Show("数据库操作失败：" + e.Message);
                        return -1;
                    }
                    catch (Exception e)
                    {
                        tx.Rollback();
                        //WriteLog.CreateLog("数据库操作日志", "DbHelperSQL类（ExecuteSqlTran函数）:", "error", e.Message);
                        MessageBox.Show("数据库操作失败：" + e.Message);
                        return -1;
                    }
                }
            }
        }
        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">多条SQL语句</param>		
        public int ExecuteSqlTran(List<String> SQLStringList, bool backupDB = false)
        {
            lock (lockObj)
            {
                using (SqlConnection conn = HelpConnection.GetConnection(backupDB))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    SqlTransaction tx = conn.BeginTransaction();
                    cmd.Transaction = tx;
                    try
                    {
                        int count = 0;
                        for (int n = 0; n < SQLStringList.Count; n++)
                        {
                            string strsql = SQLStringList[n];
                            if (strsql.Trim().Length > 1)
                            {
                                cmd.CommandText = strsql;
                                count += cmd.ExecuteNonQuery();
                            }
                        }
                        tx.Commit();
                        return count;
                    }
                    catch
                    {
                        tx.Rollback();
                        return 0;
                    }
                }
            }
        }
        /// <summary>
        /// 执行带一个存储过程参数的的SQL语句。
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <param name="content">参数内容,比如一个字段是格式复杂的文章，有特殊符号，可以通过这个方式添加</param>
        /// <returns>影响的记录数</returns>
        public int ExecuteSql(string SQLString, string content, bool backupDB = false)
        {
            lock (lockObj)
            {
                using (SqlConnection connection = HelpConnection.GetConnection(backupDB))
                {
                    SqlCommand cmd = new SqlCommand(SQLString, connection);
                    System.Data.SqlClient.SqlParameter myParameter = new System.Data.SqlClient.SqlParameter("@content", SqlDbType.NText);
                    myParameter.Value = content;
                    cmd.Parameters.Add(myParameter);
                    try
                    {
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        //WriteLog.CreateLog("数据库操作日志", "DbHelperSQL类（ExecuteSql函数）", "error", e.Message);
                        MessageBox.Show("数据库操作失败：" + e.Message);
                        return -1;
                    }
                    finally
                    {
                        cmd.Dispose();
                        connection.Close();
                    }
                }
            }
        }
        /// <summary>
        /// 执行带一个存储过程参数的的SQL语句。
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <param name="content">参数内容,比如一个字段是格式复杂的文章，有特殊符号，可以通过这个方式添加</param>
        /// <returns>影响的记录数</returns>
        public object ExecuteSqlGet(string SQLString, string content, bool backupDB = false)
        {
            lock (lockObj)
            {
                using (SqlConnection connection = HelpConnection.GetConnection(backupDB))
                {
                    SqlCommand cmd = new SqlCommand(SQLString, connection);
                    System.Data.SqlClient.SqlParameter myParameter = new System.Data.SqlClient.SqlParameter("@content", SqlDbType.NText);
                    myParameter.Value = content;
                    cmd.Parameters.Add(myParameter);
                    try
                    {
                        connection.Open();
                        object obj = cmd.ExecuteScalar();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        //WriteLog.CreateLog("数据库操作日志", "DbHelperSQL类（ExecuteSqlGet函数）:", "error", e.Message);
                        MessageBox.Show("数据库操作失败：" + e.Message);
                        return -1;
                    }
                    finally
                    {
                        cmd.Dispose();
                        connection.Close();
                    }
                }
            }
        }
        /// <summary>
        /// 向数据库里插入图像格式的字段(和上面情况类似的另一种实例)
        /// </summary>
        /// <param name="strSQL">SQL语句</param>
        /// <param name="fs">图像字节,数据库的字段类型为image的情况</param>
        /// <returns>影响的记录数</returns>
        public int ExecuteSqlInsertImg(string strSQL, byte[] fs, bool backupDB = false)
        {
            lock (lockObj)
            {
                using (SqlConnection connection = HelpConnection.GetConnection(backupDB))
                {
                    SqlCommand cmd = new SqlCommand(strSQL, connection);
                    System.Data.SqlClient.SqlParameter myParameter = new System.Data.SqlClient.SqlParameter("@fs", SqlDbType.Image);
                    myParameter.Value = fs;
                    cmd.Parameters.Add(myParameter);
                    try
                    {
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        //WriteLog.CreateLog("数据库操作日志", "DbHelperSQL类（ExecuteSqlInsertImg函数）", "error", e.Message);
                        MessageBox.Show("数据库操作失败：" + e.Message);
                        return -1;
                    }
                    finally
                    {
                        cmd.Dispose();
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public object GetSingle(string SQLString, bool backupDB = false)
        {
            lock (lockObj)
            {
                using (SqlConnection connection = HelpConnection.GetConnection(backupDB))
                {
                    using (SqlCommand cmd = new SqlCommand(SQLString, connection))
                    {
                        try
                        {
                            connection.Open();
                            object obj = cmd.ExecuteScalar();
                            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                            {
                                return null;
                            }
                            else
                            {
                                return obj;
                            }
                        }
                        catch (System.Data.SqlClient.SqlException e)
                        {
                            connection.Close();
                            //WriteLog.CreateLog("数据库操作日志", "DbHelperSQL类（GetSingle(string SQLString)函数）:", "error", e.Message);
                            MessageBox.Show("数据库操作失败：" + e.Message);
                            return -1;
                        }
                    }
                }
            }
        }
        public object GetSingle(string SQLString, int Times, bool backupDB = false)
        {
            lock (lockObj)
            {
                using (SqlConnection connection = HelpConnection.GetConnection(backupDB))
                {
                    using (SqlCommand cmd = new SqlCommand(SQLString, connection))
                    {
                        try
                        {
                            connection.Open();
                            cmd.CommandTimeout = Times;
                            object obj = cmd.ExecuteScalar();
                            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                            {
                                return null;
                            }
                            else
                            {
                                return obj;
                            }
                        }
                        catch (System.Data.SqlClient.SqlException e)
                        {
                            connection.Close();
                            //WriteLog.CreateLog("数据库操作日志", "DbHelperSQL类（GetSingle(string SQLString, int Times)函数）", "error", e.Message);
                            MessageBox.Show("数据库操作失败：" + e.Message);
                            return -1;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 执行查询语句，返回SqlDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
        /// </summary>
        /// <param name="strSQL">查询语句</param>
        /// <returns>SqlDataReader</returns>
        public SqlDataReader ExecuteReader(string strSQL, bool backupDB = false)
        {
            lock (lockObj)
            {
                SqlConnection connection = HelpConnection.GetConnection(backupDB);
                SqlCommand cmd = new SqlCommand(strSQL, connection);
                try
                {
                    connection.Open();
                    SqlDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    return myReader;
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    //WriteLog.CreateLog("数据库操作日志", "DbHelperSQL类（ExecuteReader函数）:", "error", e.Message);
                    MessageBox.Show("数据库操作失败：" + e.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public DataTable Query(string SQLString, bool backupDB = false)
        {
            lock (lockObj)
            {
                using (SqlConnection connection = HelpConnection.GetConnection(backupDB))
                {
                    DataTable dt = new DataTable();
                    try
                    {
                        connection.Open();
                        SqlDataAdapter command = new SqlDataAdapter(SQLString, connection);
                        command.Fill(dt);
                    }
                    catch (System.Data.SqlClient.SqlException ex)
                    {
                        //WriteLog.CreateLog("数据库操作日志", "DbHelperSQL类（Query(string SQLString)函数）", "error", ex.Message);
                        MessageBox.Show("数据库操作失败：" + ex.Message);
                    }
                    return dt;
                }
            }
        }

        /// <summary>
        /// 执行查询语句，返回DataTable
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataTable</returns>
        public DataTable Query(string SQLString, bool backupDB = false, params SqlParameter[] cmdParms)
        {
            lock (lockObj)
            {
                using (SqlConnection connection = HelpConnection.GetConnection(backupDB))
                {
                    SqlCommand cmd = new SqlCommand();
                    PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        try
                        {
                            da.Fill(dt);
                            cmd.Parameters.Clear();
                        }
                        catch (System.Data.SqlClient.SqlException ex)
                        {
                            //WriteLog.CreateLog("数据库操作日志", "DbHelperSQL类（Query(string SQLString, params SqlParameter[] cmdParms)函数）" + SQLString, "error", ex.Message);
                            MessageBox.Show("数据库操作失败：" + ex.Message);
                        }
                        return dt;
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="SQLString"></param>
        /// <param name="Times">查询的超时时间</param>
        /// <param name="backupDB"></param>
        /// <returns></returns>
        public DataTable Query(string SQLString, int Times, bool backupDB = false)
        {
            lock (lockObj)
            {
                using (SqlConnection connection = HelpConnection.GetConnection(backupDB))
                {
                    DataTable dt = new DataTable();
                    try
                    {
                        connection.Open();
                        SqlDataAdapter command = new SqlDataAdapter(SQLString, connection);
                        command.SelectCommand.CommandTimeout = Times;
                        command.Fill(dt);
                    }
                    catch (System.Data.SqlClient.SqlException ex)
                    {
                        //WriteLog.CreateLog("数据库操作日志", "DbHelperSQL类（Query(string SQLString, int Times)函数）", "error", ex.Message);
                        MessageBox.Show("数据库操作失败：" + ex.Message);
                    }
                    return dt;
                }
            }
        }

        #endregion

        #region 执行带参数的SQL语句

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public int ExecuteSql(string SQLString, bool backupDB = false, params SqlParameter[] cmdParms)
        {
            lock (lockObj)
            {
                using (SqlConnection connection = HelpConnection.GetConnection(backupDB))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        try
                        {
                            PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                            int rows = cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                            return rows;
                        }
                        catch (System.Data.SqlClient.SqlException e)
                        {
                            //WriteLog.CreateLog("数据库操作日志", "DbHelperSQL类（ExecuteSql(string SQLString, params SqlParameter[] cmdParms)函数）", "error", e.Message);
                            MessageBox.Show("数据库操作失败：" + e.Message);
                            return -1;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="SQLString"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public object ExecuteSqlReturn(string SQLString, bool backupDB = false, params SqlParameter[] cmdParms)
        {
            lock (lockObj)
            {
                using (SqlConnection connection = HelpConnection.GetConnection(backupDB))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        try
                        {
                            PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                            object obj = cmd.ExecuteScalar();
                            cmd.Parameters.Clear();
                            return obj;
                        }
                        catch (System.Data.SqlClient.SqlException e)
                        {
                            //WriteLog.CreateLog("数据库操作日志", "DbHelperSQL类（ExecuteSqlReturn(string SQLString, params SqlParameter[] cmdParms)函数）", "error", e.Message);
                            MessageBox.Show("数据库操作失败：" + e.Message);
                            return null;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的SqlParameter[]）</param>
        public void ExecuteSqlTran(Hashtable SQLStringList, bool backupDB = false)
        {
            lock (lockObj)
            {
                using (SqlConnection conn = HelpConnection.GetConnection(backupDB))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        SqlCommand cmd = new SqlCommand();
                        try
                        {
                            //循环
                            foreach (DictionaryEntry myDE in SQLStringList)
                            {
                                string cmdText = myDE.Key.ToString();
                                SqlParameter[] cmdParms = (SqlParameter[])myDE.Value;
                                PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                                int val = cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                            }
                            trans.Commit();
                        }
                        catch
                        {
                            trans.Rollback();

                            throw;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的SqlParameter[]）</param>
        public int ExecuteSqlTran(System.Collections.Generic.List<CommandInfo> cmdList, bool backupDB = false)
        {
            lock (lockObj)
            {
                using (SqlConnection conn = HelpConnection.GetConnection(backupDB))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        SqlCommand cmd = new SqlCommand();
                        try
                        {
                            int count = 0;
                            //循环
                            foreach (CommandInfo myDE in cmdList)
                            {
                                string cmdText = myDE.CommandText;
                                SqlParameter[] cmdParms = (SqlParameter[])myDE.Parameters;
                                PrepareCommand(cmd, conn, trans, cmdText, cmdParms);

                                if (myDE.EffentNextType == EffentNextType.WhenHaveContine || myDE.EffentNextType == EffentNextType.WhenNoHaveContine)
                                {
                                    if (myDE.CommandText.ToLower().IndexOf("count(") == -1)
                                    {
                                        trans.Rollback();
                                        return 0;
                                    }

                                    object obj = cmd.ExecuteScalar();
                                    bool isHave = false;
                                    if (obj == null && obj == DBNull.Value)
                                    {
                                        isHave = false;
                                    }
                                    isHave = Convert.ToInt32(obj) > 0;

                                    if (myDE.EffentNextType == EffentNextType.WhenHaveContine && !isHave)
                                    {
                                        trans.Rollback();
                                        return 0;
                                    }
                                    if (myDE.EffentNextType == EffentNextType.WhenNoHaveContine && isHave)
                                    {
                                        trans.Rollback();
                                        return 0;
                                    }
                                    continue;
                                }
                                int val = cmd.ExecuteNonQuery();
                                count += val;
                                if (myDE.EffentNextType == EffentNextType.ExcuteEffectRows && val == 0)
                                {
                                    trans.Rollback();
                                    return 0;
                                }
                                cmd.Parameters.Clear();
                            }
                            trans.Commit();
                            return count;
                        }
                        catch
                        {
                            trans.Rollback();
                            throw;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的SqlParameter[]）</param>
        public void ExecuteSqlTranWithIndentity(System.Collections.Generic.List<CommandInfo> SQLStringList, bool backupDB = false)
        {
            lock (lockObj)
            {
                using (SqlConnection conn = HelpConnection.GetConnection(backupDB))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        SqlCommand cmd = new SqlCommand();
                        try
                        {
                            int indentity = 0;
                            //循环
                            foreach (CommandInfo myDE in SQLStringList)
                            {
                                string cmdText = myDE.CommandText;
                                SqlParameter[] cmdParms = (SqlParameter[])myDE.Parameters;
                                foreach (SqlParameter q in cmdParms)
                                {
                                    if (q.Direction == ParameterDirection.InputOutput)
                                    {
                                        q.Value = indentity;
                                    }
                                }
                                PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                                int val = cmd.ExecuteNonQuery();
                                foreach (SqlParameter q in cmdParms)
                                {
                                    if (q.Direction == ParameterDirection.Output)
                                    {
                                        indentity = Convert.ToInt32(q.Value);
                                    }
                                }
                                cmd.Parameters.Clear();
                            }
                            trans.Commit();
                        }
                        catch
                        {
                            trans.Rollback();
                            throw;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的SqlParameter[]）</param>
        public void ExecuteSqlTranWithIndentity(Hashtable SQLStringList, bool backupDB = false)
        {
            lock (lockObj)
            {
                using (SqlConnection conn = HelpConnection.GetConnection(backupDB))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        SqlCommand cmd = new SqlCommand();
                        try
                        {
                            int indentity = 0;
                            //循环
                            foreach (DictionaryEntry myDE in SQLStringList)
                            {
                                string cmdText = myDE.Key.ToString();
                                SqlParameter[] cmdParms = (SqlParameter[])myDE.Value;
                                foreach (SqlParameter q in cmdParms)
                                {
                                    if (q.Direction == ParameterDirection.InputOutput)
                                    {
                                        q.Value = indentity;
                                    }
                                }
                                PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                                int val = cmd.ExecuteNonQuery();
                                foreach (SqlParameter q in cmdParms)
                                {
                                    if (q.Direction == ParameterDirection.Output)
                                    {
                                        indentity = Convert.ToInt32(q.Value);
                                    }
                                }
                                cmd.Parameters.Clear();
                            }
                            trans.Commit();
                        }
                        catch
                        {
                            trans.Rollback();
                            throw;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public object GetSingle(string SQLString, bool backupDB = false, params SqlParameter[] cmdParms)
        {
            lock (lockObj)
            {
                using (SqlConnection connection = HelpConnection.GetConnection(backupDB))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        try
                        {
                            PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                            object obj = cmd.ExecuteScalar();
                            cmd.Parameters.Clear();
                            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                            {
                                return null;
                            }
                            else
                            {
                                return obj;
                            }
                        }
                        catch (System.Data.SqlClient.SqlException e)
                        {
                            //WriteLog.CreateLog("数据库操作日志", "DbHelperSQL类（GetSingle(string SQLString, params SqlParameter[] cmdParms)函数）", "error", e.Message);
                            MessageBox.Show("数据库操作失败：" + e.Message);
                            return null;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 执行查询语句，返回SqlDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
        /// </summary>
        /// <param name="strSQL">查询语句</param>
        /// <returns>SqlDataReader</returns>
        public SqlDataReader ExecuteReader(string SQLString, bool backupDB = false, params SqlParameter[] cmdParms)
        {
            lock (lockObj)
            {
                SqlConnection connection = HelpConnection.GetConnection(backupDB);
                SqlCommand cmd = new SqlCommand();
                try
                {
                    PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                    SqlDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    cmd.Parameters.Clear();
                    return myReader;
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    //WriteLog.CreateLog("数据库操作日志", "DbHelperSQL类（ExecuteReader(string SQLString, params SqlParameter[] cmdParms)函数）", "error", e.Message);
                    MessageBox.Show("数据库操作失败：" + e.Message);
                    return null;
                }
            }
        }

        private void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, string cmdText, SqlParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = CommandType.Text;//cmdType;
            if (cmdParms != null)
            {
                foreach (SqlParameter parameter in cmdParms)
                {
                    if (parameter != null)
                    {
                        if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                            (parameter.Value == null))
                        {
                            parameter.Value = DBNull.Value;
                        }
                        cmd.Parameters.Add(parameter);
                    }
                }
            }
        }

        #endregion

        #region 存储过程操作

        /// <summary>
        /// 执行存储过程，返回SqlDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>SqlDataReader</returns>
        public SqlDataReader RunProcedure(string storedProcName, IDataParameter[] parameters, bool backupDB = false)
        {
            lock (lockObj)
            {
                SqlConnection connection = HelpConnection.GetConnection(backupDB);
                SqlDataReader returnReader;
                connection.Open();
                SqlCommand command = BuildQueryCommand(connection, storedProcName, parameters);
                command.CommandType = CommandType.StoredProcedure;
                returnReader = command.ExecuteReader(CommandBehavior.CloseConnection);
                return returnReader;
            }
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="tableName">DataSet结果中的表名</param>
        /// <returns>DataSet</returns>
        public DataTable RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName, out int totalPages, out int totalNumbers, bool backupDB = false)
        {
            lock (lockObj)
            {
                totalNumbers = 0;
                totalPages = 0;
                using (SqlConnection connection = HelpConnection.GetConnection(backupDB))
                {
                    DataTable dt = new DataTable();
                    connection.Open();
                    SqlCommand command = BuildIntCommand(connection, storedProcName, parameters);
                    SqlDataAdapter sqlDA = new SqlDataAdapter(command);
                    sqlDA.Fill(dt);
                    totalNumbers = (int)command.Parameters["totalRecord"].Value;
                    totalPages = (int)command.Parameters["@TotalPage"].Value;
                    connection.Close();
                    return dt;
                }
            }
        }

        public DataSet RunProcedureDatatable(string storedProcName, IDataParameter[] parameters, bool backupDB = false)
        {
            lock (lockObj)
            {
                using (SqlConnection connection = HelpConnection.GetConnection(backupDB))
                {
                    DataSet ds = new DataSet();
                    connection.Open();
                    SqlDataAdapter sqlDA = new SqlDataAdapter();
                    sqlDA.SelectCommand = BuildQueryCommand(connection, storedProcName, parameters);
                    sqlDA.Fill(ds);
                    connection.Close();
                    return ds;
                }
            }
        }

        /// <summary>
        /// 构建 SqlCommand 对象(用来返回一个结果集，而不是一个整数值)
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>SqlCommand</returns>
        private SqlCommand BuildQueryCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            SqlCommand command = new SqlCommand(storedProcName, connection);
            command.CommandType = CommandType.StoredProcedure;
            foreach (SqlParameter parameter in parameters)
            {
                if (parameter != null)
                {
                    // 检查未分配值的输出参数,将其分配以DBNull.Value.
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                        (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    command.Parameters.Add(parameter);
                }
            }

            return command;
        }

        /// <summary>
        /// 执行存储过程，返回影响的行数		
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="rowsAffected">影响的行数</param>
        /// <returns></returns>
        public int RunProcedure(string storedProcName, IDataParameter[] parameters, out int rowsAffected, bool backupDB = false)
        {
            lock (lockObj)
            {
                using (SqlConnection connection = HelpConnection.GetConnection(backupDB))
                {
                    int result;
                    connection.Open();
                    SqlCommand command = BuildIntCommand(connection, storedProcName, parameters);
                    rowsAffected = command.ExecuteNonQuery();
                    result = (int)command.Parameters["totalRecord"].Value;
                    //Connection.Close();
                    return result;
                }
            }
        }

        /// <summary>
        /// 创建 SqlCommand 对象实例(用来返回一个整数值)	
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>SqlCommand 对象实例</returns>
        private SqlCommand BuildIntCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            SqlCommand command = BuildQueryCommand(connection, storedProcName, parameters);
            command.Parameters.Add(new SqlParameter("totalRecord",
                SqlDbType.Int, 4, ParameterDirection.ReturnValue,
                false, 0, 0, string.Empty, DataRowVersion.Default, null));
            return command;
        }
        #endregion
    }
}
