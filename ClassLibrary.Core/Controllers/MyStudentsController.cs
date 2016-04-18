using System.Web.Mvc;
using ClassLibrary.Core.Service;
using ClassLibrary.Core.Models;

namespace ClassLibrary.Core.Controllers
{
    public class MyStudentsController : Controller
    { 
        // Метод, передающий в представление список студентов и предметов.
        //
        [HttpGet]
        [Authorize(Roles = "teacher")]
        public ActionResult NewTask()
        {
            // Передать в представление список студентов.
            ViewData["AllStudents"] = MyStudentsService.GetAllStudents();

            // Передать в представление список предметов.
            ViewData["AllDisciplines"] = MyStudentsService.GetAllGroups();

            return View();
        }

        // Метод, сохраняющий новое задание в базу данных.
        //
        [HttpPost]
        [Authorize(Roles = "teacher")]
        public string NewTask(StudentTask task)
        {
            MyStudentsService.AddTask(task);

            return "Задача успешно добавлена";
        }

        // Метод, передающий в представление данные о заданиях, выданных студентам.
        //
        [Authorize(Roles = "teacher")]
        public ActionResult TasksList(string userId, int? disciplineId)
        {
            // Получить данные обо всех заданиях, выданных каждому студенту.
            AllStudentsTasksListViewModel allStudentsTasksListViewModel = MyStudentsService.GetStudentsTasksList(userId, disciplineId);

            return View(allStudentsTasksListViewModel);
        }
    }
}
