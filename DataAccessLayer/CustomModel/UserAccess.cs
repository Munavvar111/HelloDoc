using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.CustomModel
{
    public class UserAccess
    {
        public string AccountType { get; set; }
        public int AccountTypeid { get; set; }
        public string AccountPOC { get; set; }

        public string phone { get; set; }
        public int roleid { get; set; }

        public int? status{ get; set; }
        public int? useraccessid { get; set; }   
        public int? OpenRequest{ get; set; }
    }
}
