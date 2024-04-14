using DataAccessLayer.CustomModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.InterFace
{
    public interface IProvider
    {
        List<NewRequestTableVM> SearchPatients(string searchValue, string selectedFilter, int[] currentStatus,string Email);
        bool RequestAcceptedByProvider(int requestId,int physicianId);
    }
}
