using System;
using System.Collections.Generic;
using System.Text;

namespace KellRole
{
    /// <summary>
    /// ��ɫ��
    /// </summary>
    [Serializable]
    public class Role : IArchitecture
    {
        /// <summary>
        /// ���캯��
        /// </summary>
        public Role()
        {
            this.permissions = new PermissionCollection();
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        public Role(int id, string name)
        {
            this.id = id;
            if (name != null && name.Trim() != "")
                this.name = name;
            this.permissions = new PermissionCollection();
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="permissions"></param>
        public Role(int id, string name, PermissionCollection permissions)
        {
            this.id = id;
            if (name != null && name.Trim() != "")
                this.name = name;
            if (permissions == null)
                this.permissions = new PermissionCollection();
            else
                this.permissions = permissions;
        }

        int id;

        /// <summary>
        /// ��ɫID
        /// </summary>
        public int ID
        {
            get { return id; }
            set { id = value; }
        }
        string name = "δ������ɫ";

        /// <summary>
        /// ��ɫ����
        /// </summary>
        public string Name
        {
            get { return name; }
            set {
                if (value != null && value.Trim() != "")
                    name = value;
            }
        }

        PermissionCollection permissions;

        /// <summary>
        /// ��ɫӵ�е�Ȩ�޼���
        /// </summary>
        public PermissionCollection Permissions
        {
            get { return permissions; }
            set
            {
                if (value != null)
                    permissions = value;
            }
        }
        bool flag = true;

        /// <summary>
        /// ����״̬(Ĭ�ϣ�true����)
        /// </summary>
        public bool Flag
        {
            get { return flag; }
            set { flag = value; }
        }

        string remark;
        /// <summary>
        /// ��ɫ����
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
            return this.Name;
        }
    }
}
