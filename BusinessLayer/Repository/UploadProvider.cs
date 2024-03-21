using BusinessLayer.InterFace;
using DataAccessLayer.DataContext;
using Microsoft.AspNetCore.Http;

namespace BusinessLayer.Repository
{
    public class UploadProvider:IUploadProvider
    {
        private readonly ApplicationDbContext _context;
        public  UploadProvider(ApplicationDbContext context)
        {
            _context = context;
        }
        public string UploadSignature(IFormFile SignName, int physicianid)
        {

            string folderPath = "wwwroot\\Physician\\" + physicianid;
            string path = Path.Combine(Directory.GetCurrentDirectory(), folderPath);
            string fileExtension = Path.GetExtension(SignName.FileName);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string fileName = "signature" + (fileExtension==""?".png":fileExtension);
            string filePath = Path.Combine(path, fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                SignName.CopyTo(fileStream);
            }
            return fileName;
        }
        public string UploadPhoto(IFormFile PhotoName, int physicianid)
        {

            string folderPath = "wwwroot\\Physician\\" + physicianid;
            string path = Path.Combine(Directory.GetCurrentDirectory(), folderPath);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string fileExtension = Path.GetExtension(PhotoName.FileName);
            string fileName = "photo" + fileExtension;
            string filePath = Path.Combine(path, fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                PhotoName.CopyTo(fileStream);
            }
            return fileName;
        }
        public string UploadDocFile(IFormFile PhotoName, int physicianid, string FileName)
        {

            string folderPath = "wwwroot\\Physician\\" + physicianid;
            string path = Path.Combine(Directory.GetCurrentDirectory(), folderPath);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string fileExtension = Path.GetExtension(PhotoName.FileName);
            string fileName = FileName + fileExtension;
            string filePath = Path.Combine(path, fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                PhotoName.CopyTo(fileStream);
            }
            return fileName;
        }
    }
}
