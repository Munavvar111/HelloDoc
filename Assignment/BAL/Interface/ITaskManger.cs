using DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task = DAL.DataModels.Task;


namespace BAL.Interface
{
    public interface ITaskManger
    {
        List<Task> GetTaskDetails(string searchValue);

        List<Category> GetAllCategories();

         bool AddTask(string Taskname, string Asignee, string Discription, DateTime DueDate , string City, int Category);

        bool DeleteTask(int TaskId); 


        bool UpdateTask(string TaskName, string Asignee, string Discription, DateTime DueDate, string City, string Category, int TaskId); 

        Task GetTaskById(int TaskId);
    }
}
