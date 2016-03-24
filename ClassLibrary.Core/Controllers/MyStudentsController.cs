using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using ClassLibrary.Core.Models;

namespace ClassLibrary.Core.Controllers
{
    public class MyStudentsController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        [HttpGet]
        public ActionResult NewTask()
        {
            var students = db.Users;
            ViewData["AllStudents"] = from User in db.Users select new SelectListItem { Text = User.LastName + " " + User.FirstName, Value = User.Id.ToString() };

            var disciplines = db.Disciplines;
            ViewData["AllDisciplines"] = from Discipline in db.Disciplines select new SelectListItem { Text = Discipline.Name, Value = Discipline.DisciplineId.ToString() };

            return View();
        }

        [HttpPost]
        public string NewTask(StudentTask task)
        {
            db.StudentTasks.Add(task);

            db.SaveChanges();

            return "Задача успешно добавлена";
        }
    }
}
