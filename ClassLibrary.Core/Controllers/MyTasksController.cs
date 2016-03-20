using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using ClassLibrary.Core.Models;
using Microsoft.AspNet.Identity;

namespace ClassLibrary.Core.Controllers
{
    public class MyTasksController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index(int? disciplineId)
        {
            IQueryable<StudentTask> studentTasks = db.StudentTasks.Include("Discipline");

            string currentUserId = User.Identity.GetUserId();

            studentTasks = studentTasks.Where(p => p.UserId == currentUserId);

            if (disciplineId != null && disciplineId != 0)
            {
                studentTasks = studentTasks.Where(p => p.DisciplineId == disciplineId);
            }

            List<Discipline> disciplines = db.Disciplines.ToList();//список дисциплин

            disciplines.Insert(0, new Discipline { Name = "Все", DisciplineId = 0 });//выбрать все задания

            MyTasksListViewModel mtlvm = new MyTasksListViewModel //данные, которые будут переданы в представление
            {
                StudentTasks = studentTasks.ToList(),
                Disciplines = new SelectList(disciplines, "DisciplineId","name")
            };

            return View(mtlvm);
        }
    }
}