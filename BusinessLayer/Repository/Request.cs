using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.InterFace;
using DataAccessLayer.DataContext;

namespace BusinessLayer.Repository
{
    public class Request : IRequest
    {
        private readonly ApplicationDbContext _context;

        public Request(ApplicationDbContext context) {
        _context = context;
        }
    }
}
