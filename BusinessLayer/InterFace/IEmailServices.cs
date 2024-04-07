using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.InterFace
{
	public interface IEmailServices
	{
		public bool IsSendEmail(string toEmail, string subject, string body, List<string> filenames);

	}
}
