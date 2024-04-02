using DataAccessLayer.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.CustomModel
{
	public class ProviderOnCallVM
	{
		public List<Physician>? OffDuty { get; set; }

		public List<Physician>? OnDuty { get; set; }	
		public List<Region>? Regions { get; set; }	
	}
}
