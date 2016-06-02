using System;
using System.Collections.Generic;
using System.Text;

namespace KellRole
{
    /// <summary>
    /// ������
    /// </summary>
    [Serializable]
    public class Action : IArchitecture
    {
        int id;

        /// <summary>
        /// ����ID
        /// </summary>
        public int ID
        {
            get { return id; }
            set { id = value; }
        }
        string name = "δ��������";

        /// <summary>
        /// ����������
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
        /// ����������
        /// </summary>
        /// </summary>
        public string Remark
        {
            get { return remark; }
            set { remark = value; }
        }

        /// <summary>
        /// Ĭ�Ϲ��캯��
        /// </summary>
        public Action()
        {
        }

        /// <summary>
        /// ��ָ�������ƹ����������
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        public Action(int id, string name)
        {
            this.id = id;
            if (name != null && name.Trim() != "")
                this.name = name;
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Name;
        }
    }
}
