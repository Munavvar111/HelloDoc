using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.CustomModel;

namespace BusinessLayer.InterFace
{
    public interface IAdmin
    {   
        List<NewRequestTableVM> SearchPatients(string searchValue, string selectValue, string selectedFilter,int[] currentStatus);

    }
}
