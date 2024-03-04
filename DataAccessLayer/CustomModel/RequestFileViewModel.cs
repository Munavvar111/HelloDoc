using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.DataModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DataAccessLayer.CustomModel
{
    public class RequestFileViewModel
    {
        public User? User { get; set; } 

        public Request? Request { get; set; }   
        public int? Requestid {  get; set; }
        public List<Requestwisefile>? Requestwisefileview { get; set; }
        [FromForm]
        public IFormFile? File { get; set; }

        public BitArray? IsDeleted { get; set; } 
    }
}
