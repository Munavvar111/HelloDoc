using DataAccessLayer.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.CustomModel
{
    public class SendOrderModel
    {

        public int requestid {  get; set; } 
        public List<Healthprofessional>? helthProfessional { get; set; } 

        public List<Healthprofessionaltype>? Healthprofessionaltypes { get; set; }   
    }
}
