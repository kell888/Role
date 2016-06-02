using System;
using System.Collections.Generic;
using System.Text;

namespace KellRole
{
    /// <summary>
    /// ҵ��ģ����
    /// </summary>
    [Serializable]
    public class Module : IArchitecture
    {
        int id;

        /// <summary>
        /// ģ��ID
        /// </summary>
        public int ID
        {
            get { return id; }
            set { id = value; }
        }
        string name = "δ����ģ��";

        /// <summary>
        /// ģ�������
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
        string formName;

        /// <summary>
        /// ������
        /// </summary>
        public string FormName
        {
            get { return formName; }
            set { formName = value; }
        }
        string controlName;

        /// <summary>
        /// �ؼ���
        /// </summary>
        public string ControlName
        {
            get { return controlName; }
            set { controlName = value; }
        }

        string remark;

        /// <summary>
        /// ģ�������
        /// </summary>
        public string Remark
        {
            get { return remark; }
            set { remark = value; }
        }

        /// <summary>
        /// Ĭ�Ϲ��캯��
        /// </summary>
        public Module()
        {
        }

        /// <summary>
        /// ��ָ�������ƹ���ģ�����
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        public Module(int id, string name)
        {
            this.id = id;
            if (name != null && name.Trim() != "")
                this.name = name;
        }

        /// <summary>
        /// ģ������
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Name;
        }
    }
}
