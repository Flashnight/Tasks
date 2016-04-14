using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ClassLibrary.Core.Models;
using Microsoft.AspNet.Identity;
using System.Web;
using System.IO;
using System;

namespace ClassLibrary.Core.Controllers
{
    public class MyTasksController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        [Authorize(Roles = "student")]
        public ActionResult List(int? disciplineId)
        {
            IQueryable<StudentTask> studentTasks = db.StudentTasks.Include("Discipline");

            string currentUserId = User.Identity.GetUserId();   //id текущего пользователя

            studentTasks = studentTasks.Where(p => p.UserId == currentUserId);  //получить все задачи пользователя

            if (disciplineId != null && disciplineId != 0)
            {
                studentTasks = studentTasks.Where(p => p.DisciplineId == disciplineId); //отфильтровать задания по выбранной дисциплине
            }

            List<Discipline> disciplines = db.Disciplines.ToList();//список дисциплин

            disciplines.Insert(0, new Discipline { Name = "Все дисциплины", DisciplineId = 0 });   //выбрать все задания

            MyTasksListViewModel mtlvm = new MyTasksListViewModel   //данные, которые будут переданы в представление
            {
                StudentTasks = studentTasks.ToList(),
                Disciplines = new SelectList(disciplines, "DisciplineId","name")
            };

            return View(mtlvm);
        }

        [Authorize(Roles = "student")]
        public ActionResult Description(int taskId)
        {
            StudentTask task = db.StudentTasks.FirstOrDefault<StudentTask>(p => p.StudentTaskId == taskId); //найти задачу по id в базе данных

            ViewBag.Task = task;    //передать задания в представление

            return View();
        }

        [Authorize(Roles = "student")]
        [HttpPost]
        public ActionResult UploadSolution(int taskId, HttpPostedFileBase upload)
        {
            if(upload!=null)
            {
                //Присвоить файлу случайное имя через Guid
                string fileName = string.Format("{0}.{1}", Guid.NewGuid(), Path.GetExtension(upload.FileName));

                //Сохранение файла решения в папку Solutions
                upload.SaveAs(Server.MapPath("~/Solutions/" + fileName));

                //Сохранение ссылки на файл в базе данных
                Solution NewSolution = new Solution { Path = fileName, StudentTaskId = taskId };
                db.Solutions.Add(NewSolution);
                db.SaveChanges();
            }

            //Перенаправить на список заданий
            return RedirectToAction("List");
        }
    }
}