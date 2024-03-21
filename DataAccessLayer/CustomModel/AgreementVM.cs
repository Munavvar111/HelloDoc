using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.CustomModel
{
    public class AgreementVM
    {
        public int RequestId { get; set; }  
        public int status { get; set; } 
        public string? Notes { get; set; }  
     }
}
