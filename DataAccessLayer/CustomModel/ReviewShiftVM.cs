using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.CustomModel
{
    public  class ReviewShiftVM
    {
        
            public int Shiftid { get; set; }
            public int ShiftDetailId { get; set; }
            public string Firstname { get; set; }
            public string Lastname { get; set; }
            public DateTime Shiftdate { get; set; }
            public TimeOnly Starttime { get; set; }
            public TimeOnly Endtime { get; set; }
            public int Regionid { get; set; }
            public string RegionName { get; set; }
            public int Status { get; set; }
    }
}
