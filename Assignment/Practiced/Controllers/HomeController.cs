using BAL.Interface;
using BAL.Repository;
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
        private readonly ITaskManger _taskmanger;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, ITaskManger taskManger)
        {
            _logger = logger;
            _context = context;
            _taskmanger = taskManger;
        }

        public IActionResult Index()
        {
            //Pass Categorys In Database
            ViewBag.Categorys = _taskmanger.GetAllCategories();
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

        /// <summary>
        /// GetAll Task In Main Page In It Will Render In partialVIew TaskManagement Partial
        /// </summary>
        public IActionResult GetTaskDetails(string searchValue, int page = 1)
        {
            //Get All Categorys From The Repositry
            ViewBag.Categorys = _taskmanger.GetAllCategories();

            //For Pagination The Table
            var DetailsTask = _taskmanger.GetTaskDetails(searchValue);
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
        public IActionResult AddTaskDetails(string Taskname, string Asignee, string Discription, DateTime DueDate, string City, int Category)
        {
            //Add Task In DataBase
            if (_taskmanger.AddTask(Taskname, Asignee, Discription, DueDate, City, Category))
            {
                return Ok();
            }
            else
            {
                //Show Paasedd Message In Javascript And Handle that in javascript
                return Ok();
            }
        }
        [HttpPost]
        public IActionResult EditTaskDetails(string Taskname, string Asignee, string Discription, DateTime DueDate, string City, string Category, int TaskID)
        {
            //Edit The Task Details Using Modal
            if(_taskmanger.UpdateTask(Taskname,Asignee,Discription,DueDate,City,Category,TaskID))
            {
            return Ok();
                
            }
            return Ok();
        }
        public IActionResult DeleteTask(int taskid)
        {
            //Delete The Task
            if (_taskmanger.DeleteTask(taskid))
            {
                return RedirectToAction("Index");
            }
            else{
                return RedirectToAction("Index");
            }
        }

        public IActionResult TaskDetailsFromId(int item)
        {
            //Fetch Task Details On EditTask Modal
            //Here Item Is TaskId
            var task = _taskmanger.GetTaskById(item);
            return Json(task);
        }
    }
}