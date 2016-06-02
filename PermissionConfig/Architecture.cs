using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KellRole;

namespace PermissionConfig
{
    [Serializable]
    public class Architecture
    {
        List<User> users;

        public List<User> Users
        {
            get { return users; }
            set { users = value; }
        }
        List<UserGroup> ugrps;

        public List<UserGroup> Ugroups
        {
            get { return ugrps; }
            set { ugrps = value; }
        }
        List<Department> deps;

        public List<Department> Deps
        {
            get { return deps; }
            set { deps = value; }
        }
        List<Role> roles;

        public List<Role> Roles
        {
            get { return roles; }
            set { roles = value; }
        }
        List<Permission> pers;

        public List<Permission> Pers
        {
            get { return pers; }
            set { pers = value; }
        }
        List<Module> mods;

        public List<Module> Mods
        {
            get { return mods; }
            set { mods = value; }
        }
        List<KellRole.Action> acts;

        public List<KellRole.Action> Acts
        {
            get { return acts; }
            set { acts = value; }
        }

        public Architecture()
        {
            new Architecture(false);
        }

        private Architecture(bool empty = false)
        {
            if (!empty)
            {
                pers = new List<Permission>();
                roles = new List<Role>();
                users = new List<User>();
                ugrps = new List<UserGroup>();
                deps = new List<Department>();
                mods = new List<Module>();
                acts = new List<KellRole.Action>();
            }
        }

        public static Architecture Empty
        {
            get
            {
                return new Architecture(true);
            }
        }
    }
}
