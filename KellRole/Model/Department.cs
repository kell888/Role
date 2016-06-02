using System;
using System.Collections.Generic;
using System.Text;

namespace KellRole
{
    /// <summary>
    /// ������
    /// </summary>
    [Serializable]
    public class Department : IArchitecture
    {
        /// <summary>
        /// Ĭ�Ϲ��캯��
        /// </summary>
        public Department()
        {
            roles = new RoleCollection();
        }

        /// <summary>
        /// �����ƴ������Ŷ���
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        public Department(int id, string name)
        {
            this.id = id;
            if (name != null && name.Trim() != "")
                this.name = name;
            roles = new RoleCollection();
        }

        /// <summary>
        /// �����ƺͽ�ɫ���ϴ������Ŷ���
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="roles"></param>
        public Department(int id, string name, RoleCollection roles)
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
        /// �����ƺ��ϼ����Ŵ����²��Ŷ���
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        public Department(int id, string name, int parent)
        {
            this.id = id;
            if (name != null && name.Trim() != "")
                this.name = name;
            this.parentId = parent;
            roles = new RoleCollection();
        }

        /// <summary>
        /// �����ơ���ɫ���Ϻ��ϼ����Ŵ����²��Ŷ���
        /// </summary>
        /// <param name="name"></param>
        /// <param name="roles"></param>
        /// <param name="parent"></param>
        public Department(int id, string name, RoleCollection roles, int parent)
        {
            this.id = id;
            if (name != null && name.Trim() != "")
                this.name = name;
            if (roles == null)
                this.roles = new RoleCollection();
            else
                this.roles = roles;
            this.parentId = parent;
        }

        /// <summary>
        /// ��ָ�������ƴ����Ӳ���
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public Department CreateSubDepartment(int id, string name)
        {
            return new Department(id, name, this.ID);
        }

        /// <summary>
        /// ��ָ�������ƴ����Ӳ���
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public Department CreateSubDepartment(int id, string name, RoleCollection roles)
        {
            return new Department(id, name, roles, this.ID);
        }

        /// <summary>
        /// ָ�������Ƿ�Ϊ�ҵĺ��
        /// </summary>
        /// <param name="dep">ָ������</param>
        /// <param name="recursion">�Ƿ�ݹ��жϺ����Ĭ��Ϊtrue��ֻ����һ��Ϊfalse</param>
        /// <returns></returns>
        public bool IsMyChildren(Department dep, bool recursion = true)
        {
            if (dep.Parent != null)
            {
                if (dep.Parent.ID == this.ID)//dep=1.2,this=1
                    return true;//dep.parent=1

                if (recursion)//dep=1.2.3
                {
                    return this.IsMyChildren(dep.Parent, true);//dep=1.2.3,this=1.2
                }
            }
            return false;
        }

        int id;

        /// <summary>
        /// ����ID
        /// </summary>
        public int ID
        {
            get { return id; }
            set { id = value; }
        }
        RoleCollection roles;

        /// <summary>
        /// ����ӵ�еĽ�ɫ����
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
        string name = "δ��������";

        /// <summary>
        /// ��������
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
        string manager;

        public string Manager
        {
            get { return manager; }
            set { manager = value; }
        }

        int parentId;

        /// <summary>
        /// �ϼ�����
        /// </summary>
        public int ParentID
        {
            get { return parentId; }
            set { parentId = value; }
        }

        public Department Parent
        {
            get
            {
                if (parentId > 0)
                {
                    Department d = new Department();
                    d.ID = parentId;
                    return d;
                }
                return null;
            }
        }
        string remark;

        /// <summary>
        /// ��������
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

    public static class DepartmentListExtensions
    {
        public static bool ContainsDepartment(this List<Department> deps, Department dep)
        {
            foreach (Department d in deps)
            {
                if (d.ID == dep.ID)
                    return true;
            }
            return false;
        }

        public static bool ContainsDepartment(this List<int> deps, int dep)
        {
            return deps.Contains(dep);
        }
    }
}
