using DAL.DataContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Practiced.Models;
using System.Diagnostics;
using System.Drawing.Printing;
using Task = DAL.DataModels.Task;

namespace Practiced.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger,ApplicationDbContext context)
        {
            _logger = logger;
            _context = context; 
        }

        public IActionResult Index()
        {
            ViewBag.Categorys=_context.Categories.ToList(); 
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult GetTaskDetails(string searchValue,int page=1)
        {
            ViewBag.Categorys = _context.Categories.ToList();

            var DetailsTask =_context.Tasks.Include(req=>req.CategoryNavigation).Where(item=>string.IsNullOrEmpty(searchValue) || item.TaskName.Contains(searchValue)).ToList();
            int totalItems = DetailsTask.Count();
            int pageSize = 5;
            //Count TotalPage
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            var paginatedData = DetailsTask.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.totalPages = totalPages;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;

            return PartialView("TaskManagementPartial", paginatedData);
        }

        [HttpPost]
        public IActionResult AddTaskDetails(string Taskname,string Asignee,string Discription,DateTime DueDate,string City,int Category)
        {
            Task task = new Task();
            task.TaskName = Taskname;   
            task.Assignee = Asignee;
            task.Description= Discription;  
            //task.Category = Category;
            task.DueDate = DueDate;
            task.CategoryId = Category;
            task.Category = _context.Categories.Find(Category).CategoryName;
            task.City=City;
            _context.Tasks.Add(task);
            _context.SaveChanges();
            return Ok();
        }
        [HttpPost]
        public IActionResult EditTaskDetails(string Taskname,string Asignee,string Discription,DateTime DueDate,string City,string Category,int TaskID)
        {
            var task = _context.Tasks.Find(TaskID);
            task.TaskName = Taskname;   
            task.Assignee = Asignee;
            task.Category = Category;
            task.DueDate = DueDate;
            task.City=City;
            _context.Tasks.Update(task);
            _context.SaveChanges();
            return Ok();
        }
        public IActionResult DeleteTask(int taskid)
        {
            var task = _context.Tasks.Find(taskid);
            _context.Tasks.Remove(task);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult TaskDetailsFromId(int item)
        {
            var task=_context.Tasks.Find(item);
            return Json(task);
        }
    }
}