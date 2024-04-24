using BAL.Interface;
using DAL.DataContext;
using Microsoft.EntityFrameworkCore;    
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Task = DAL.DataModels.Task;
using System.Threading.Tasks;
using DAL.DataModels;
using Microsoft.VisualBasic;

namespace BAL.Repository
{
    public class TaskManegerRepo:ITaskManger
    {
        private readonly ApplicationDbContext _context;

        public TaskManegerRepo(ApplicationDbContext context)
        {
            _context = context;
        }   

        public List<Task> GetTaskDetails(string searchValue)
        {
            return _context.Tasks.Include(req => req.CategoryNavigation).Where(item => string.IsNullOrEmpty(searchValue) || item.TaskName.Contains(searchValue)).ToList();
        }

        public List<Category> GetAllCategories()
        {
            return _context.Categories.ToList();
        }

        public bool AddTask(string Taskname,string Asignee,string Discription,DateTime DueDate,string City, int Category)
        {
            Task task = new Task();
            task.TaskName = Taskname;
            task.Assignee = Asignee;
            task.Description = Discription;
            task.DueDate = DueDate;
            task.CategoryId = Category;
            task.Category = _context.Categories.Find(Category).CategoryName;
            task.City = City;
            _context.Tasks.Add(task);
            _context.SaveChanges();
            return true;
        }
        public bool DeleteTask(int TaskId) {
            var task = _context.Tasks.Find(TaskId);
            _context.Tasks.Remove(task);
            _context.SaveChanges();
            return true;
        }
        public bool UpdateTask(string TaskName, string Asignee, string Discription, DateTime DueDate, string City, string Category, int TaskId)
        {

            var task = _context.Tasks.Find(TaskId);
            task.TaskName = TaskName;
            task.Assignee = Asignee;
            task.Category = Category;
            task.DueDate = DueDate;
            task.City = City;
            _context.Tasks.Update(task);
            _context.SaveChanges(); return true;
        }
    }
}
