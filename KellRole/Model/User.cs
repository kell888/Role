using System;
using System.Collections.Generic;
using System.Text;

namespace KellRole
{
    /// <summary>
    /// �û���
    /// </summary>
    [Serializable]
    public class User : IArchitecture
    {
        int id;

        /// <summary>
        /// �û�ID
        /// </summary>
        public int ID
        {
            get { return id; }
            set { id = value; }
        }
        List<Department> departments;

        /// <summary>
        /// ���ڲ���ID
        /// </summary>
        public List<Department> Departments
        {
            get { return departments; }
            set
            {
                if (value != null)
                    departments = value;
            }
        }
        List<UserGroup> userGroups;

        /// <summary>
        /// �����û���
        /// </summary>
        public List<UserGroup> Usergroups
        {
            get { return userGroups; }
            set
            {
                if (value != null)
                    userGroups = value;
            }
        }

        RoleCollection roles;

        /// <summary>
        /// �û�ӵ�еĽ�ɫ����
        /// </summary>
        public RoleCollection Roles
        {
            get { return roles; }
            set
            {
                if (value != null)
                    roles = value;
            }
        }
        string userName = "δ�����û�";

        /// <summary>
        /// �û���
        /// </summary>
        public string Username
        {
            get { return userName; }
            set
            {
                if (value != null && value.Trim() != "")
                    userName = value;
            }
        }
        string password;

        /// <summary>
        /// ����
        /// </summary>
        public string Password
        {
            get { return password; }
            set { password = value; }
        }
        int flag;

        /// <summary>
        /// ״̬
        /// </summary>
        public int Flag
        {
            get { return flag; }
            set { flag = value; }
        }
        string remark;

        /// <summary>
        /// �û�����
        /// </summary>
        public string Remark
        {
            get { return remark; }
            set { remark = value; }
        }

        /// <summary>
        /// ���ػ����ToString()
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Username;
        }

        /// <summary>
        /// �û���Ĭ�Ϲ��캯��
        /// </summary>
        public User()
        {
            departments = new List<Department>();
            roles = new RoleCollection();
            userGroups = new List<UserGroup>();
        }

        /// <summary>
        /// ���û��������빹���û�����
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public User(int id, string userName, string password)
        {
            this.id = id;
            departments = new List<Department>();
            roles = new RoleCollection();
            userGroups = new List<UserGroup>();
            if (userName != null && userName.Trim() != "")
                this.userName = userName;
            this.password = password;
        }

        /// <summary>
        /// ���û�������������ڲ���ID�����û�����
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="departId"></param>
        public User(int id, string userName, string password, Department depart)
        {
            this.id = id;
            this.departments = new List<Department>();
            roles = new RoleCollection();
            userGroups = new List<UserGroup>();
            if (userName != null && userName.Trim() != "")
                this.userName = userName;
            this.password = password;
            this.departments.Add(depart);
        }
    }
}
