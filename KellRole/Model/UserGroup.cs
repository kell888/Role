using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;

namespace KellRole
{
    /// <summary>
    /// ��ʾ��User������ɵļ���
    /// </summary>
    [Serializable]
    public class UserGroup : IArchitecture//, ICollection
    {
        /// <summary>
        /// ���캯��
        /// </summary>
        public UserGroup()
        {
            roles = new RoleCollection();
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        public UserGroup(int id, string name)
        {
            this.id = id;
            if (name != null && name.Trim() != "")
                this.name = name;
            roles = new RoleCollection();
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="roles"></param>
        public UserGroup(int id, string name, RoleCollection roles)
        {
            this.id = id;
            if (name != null && name.Trim() != "")
                this.name = name;
            if (roles == null)
                this.roles = new RoleCollection();
            else
                this.roles = roles;
        }

        /// <summary>
        /// ���ػ����ToString()
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Name;
        }

        int id;

        /// <summary>
        /// �û���ID
        /// </summary>
        public int ID
        {
            get { return id; }
            set { id = value; }
        }
        RoleCollection roles;

        /// <summary>
        /// �û���ӵ�еĽ�ɫ����
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

        string name = "δ�����û���";

        /// <summary>
        /// �û��������
        /// </summary>
        public string Name
        {
            get { return name; }
            set
            {
                if (value != null && value.Trim() != "")
                    name = value;
            }
        }
        string remark;

        /// <summary>
        /// �û�������
        /// </summary>
        public string Remark
        {
            get { return remark; }
            set { remark = value; }
        }
    }

    public static class UserGroupListExtensions
    {
        public static bool ContainsUserGroup(this List<UserGroup> ugs, UserGroup ug)
        {
            foreach (UserGroup u in ugs)
            {
                if (u.ID == ug.ID)
                    return true;
            }
            return false;
        }

        public static bool ContainsUserGroup(this List<int> ugs, int ug)
        {
            return ugs.Contains(ug);
        }
    }
}
