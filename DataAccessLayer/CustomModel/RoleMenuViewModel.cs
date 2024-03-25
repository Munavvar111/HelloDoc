using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.DataModels;

namespace DataAccessLayer.CustomModel
{
    public class RoleMenuViewModel
    {
        public List<Menu> MenuList { get; set; }
        public List<int> RoleMenuIds { get; set; }
    }
}
