using DataAccessLayer.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.CustomModel
{
    public class ProviderVM
    {
        public string Name { get; set; } = null!;
        
        public string Role { get; set; } = null!;   

        public BitArray? OnCallStaus {  get; set; }   
        public BitArray? IsNotificationStoped {  get; set; }   

        public short? status { get; set; }
        public List<Region> regions { get; set; } = null!;
        public Int32 physicianid { get; set; }    
    }
}
