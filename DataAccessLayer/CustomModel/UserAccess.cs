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

        public string AccountPOCUser { get; set; }
        public string AccountPOCAdmin { get; set; }
        public string AccountPOCPhy { get; set; }

        public string phone { get; set; }
        public string phonePhy { get; set; }
        public string phoneAdmin { get; set; }
        public int roleid { get; set; }

        public int? physicianid { get; set; }    
        public int? statusUser { get; set; }
        public int? statusPhy { get; set; }
        public int? statusAdmin { get; set; }
        public int? adminid { get; set; }   
        public int? OpenRequest{ get; set; }
    }
}
