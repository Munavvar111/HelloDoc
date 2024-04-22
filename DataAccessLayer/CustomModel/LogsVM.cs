using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.CustomModel
{
    public class LogsVM
    {
        public string? Recipient { get; set; }

        public int? Action { get; set; }

        public string? RoleName { get; set; }

        public string? MobileNumber { get; set; }

        public string? Email { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? SentDate { get; set; }

        public bool? Sent { get; set; }

        public int? SentTries { get; set; }

        public string? ConfirmationNumber { get; set; }

        public bool isSms { get; set; }
            public int AccountType { get; set; }    


    }
}
