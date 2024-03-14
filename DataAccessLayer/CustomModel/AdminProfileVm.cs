using DataAccessLayer.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.CustomModel
{
    public class AdminProfileVm
    {

        public string Username { get; set; }
        public string Password { get; set; }
        public string Status { get; set; }
        public string Role { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ConfirmEmail { get; set; }
        public string MobileNo { get; set; }

        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public int? State { get; set; }
        public string ZipCode { get; set; }
        public List<Region>? Regions { get; set; }
        public List<AdminRegion>? WorkingRegions { get; set; }

    }


}
