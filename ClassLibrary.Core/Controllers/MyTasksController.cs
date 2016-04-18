using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using ClassLibrary.Core.Models;
using ClassLibrary.Core.Service;

namespace ClassLibrary.Core.Controllers
{
    public class MyTasksController : Controller
    {
        // Метод, передающий список заданий студента в представление.
        //
        [Authorize(Roles = "student")]
        public ActionResult List(int? disciplineId)
        {
            // Id текущего пользователя.
            string currentUserId = User.Identity.GetUserId();   

            // Получить список заданий из базы данных.
            MyTasksListViewModel myTasksListViewModel = MyTasksService.GetMyTaskList(disciplineId,currentUserId);

            return View(myTasksListViewModel);
        }

        // Метод, передающий информацию о задании в представление.
        //
        [Authorize(Roles = "student")]
        public ActionResult Description(int taskId)
        {
            // Получить информацию о задании из бд.
            StudentTask task = MyTasksService.GetMyTask(taskId);

            //передать задания в представление.
            ViewBag.Task = task;    

            return View();
        }

        // Метод, загружающий файл решения задания в папку Solutions проекта
        // и сохраняющий имя файла в базу данных.
        //
        [Authorize(Roles = "student")]
        [HttpPost]
        public ActionResult UploadSolution(int taskId, HttpPostedFileBase upload)
        {
            // Если указан файл, то загрузить его на сервер.
            //
            if(upload!=null)
            {
                // Получить физический путь к папке с решениями.
                string path = Server.MapPath("~/Solutions/");

                // Сохранить файл решения в файловой системе.
                MyTasksService.UploadSolution(taskId, upload, path);
            }

            //Перенаправить на список заданий.
            return RedirectToAction("List");
        }
    }
}