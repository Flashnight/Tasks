﻿using System.Linq;
using System.Web.Mvc;
using ClassLibrary.Core.Models;
using System.Collections.Generic;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace ClassLibrary.Core.Controllers
{
    public class MyStudentsController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        
        // Метод, передающий в представление список студентов и предметов
        //
        [HttpGet]
        [Authorize(Roles = "teacher")]
        public ActionResult NewTask()
        {
            ApplicationUserManager userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

            // Передать в представление список студентов
            //
            IEnumerable<SelectListItem> students = db.Users.Select(s => new SelectListItem { Text = s.LastName + " " + s.FirstName, Value = s.Id });
            // Отфильтровать список пользователей, оставив только студентов
            students = students.Where(s => userManager.GetRoles(s.Value).Contains("student"));
            ViewData["AllStudents"] = students;

            // Передать в представление список предметов
            //
            IEnumerable<SelectListItem> disciplines = db.Disciplines.Select(s => new SelectListItem { Text = s.Name, Value = s.DisciplineId.ToString() });
            ViewData["AllDisciplines"] = disciplines;

            return View();
        }

        // Метод, сохраняющий новое задание в базу данных
        //
        [HttpPost]
        [Authorize(Roles = "teacher")]
        public string NewTask(StudentTask task)
        {
            db.StudentTasks.Add(task);

            db.SaveChanges();

            return "Задача успешно добавлена";
        }

        // Метод, передающий в представление данные о заданиях, выданных студентам.
        //
        [Authorize(Roles = "teacher")]
        public ActionResult TasksList(string userId, int? disciplineId)
        {
            // Получить данные из таблиц базы данных: StudentTasks, Disciplines и AspNetUsers.
            IQueryable<StudentTask> studentTasks = db.StudentTasks.Include("Discipline").Include("User");

            // Отфильтровать задания по выбранной дисциплине, если та указана.
            //
            if (disciplineId != null && disciplineId != 0)
            {
                studentTasks = studentTasks.Where(p => p.DisciplineId == disciplineId); 
            }

            // Отфильтровать задания по id студента, если тот указан.
            //
            if (userId != null && userId != "AllStudents")
            {
                studentTasks = studentTasks.Where(p => p.UserId == userId);
            }

            // Сохранить список дисциплин для выпадающего списка.
            List<Discipline> disciplines = db.Disciplines.ToList();

            // Вставить значение "все дисциплины" для выпадающего списка.
            disciplines.Insert(0, new Discipline { Name = "Все дисциплины", DisciplineId = 0 });

            // Сформировать список пользователей в формате "имя фамилия" для выпадающего списка.
            var students = db.Users.Select(s => new { Id = s.Id, Name = s.LastName + " " + s.FirstName }).ToList();

            // Отфильтровать пользователей и сохранить в списке только студентов.
            //
            ApplicationUserManager userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            students = students.Where(s => userManager.GetRoles(s.Id).Contains("student")).ToList();

            // Вставить значение "все студенты для выпадающего списка.
            students.Insert(0, new { Id = "AllStudents", Name = "Все студенты" });

            // Модель структуры данных, передаваемая в представление.
            //
            AllStudentsTasksListViewModel allStudentsTasksListViewModel = new AllStudentsTasksListViewModel
            {
                StudentTasks = studentTasks.ToList(),
                Disciplines = new SelectList(disciplines, "DisciplineId", "Name"),
                Students = new SelectList(students, "Id", "Name")
            };

            // Вернуть представление.
            return View(allStudentsTasksListViewModel);
        }
    }
}
