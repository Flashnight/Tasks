//-----------------------------------------------------------------------
// <copyright file = "MyStudentsController.cs" company="OmSTU">
// Copyright (c) OmSTU. All rights reserved.
// </copyright>
// <author> Рудгальский Михаил </author>
// <author> Федоров Виталий </author>
// <author> Денисов Олег </author>
//-----------------------------------------------------------------------

namespace ClassLibrary.Core.Controllers
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.IO;
    using System.Linq;
    using System.Net.Mime;
    using System.Web;
    using System.Web.Mvc;
    using ClassLibrary.Core.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;

    /// <summary>
    /// Предоставляет контроллер с методами для работы с данными о студентах и выполненных ими заданиях.
    /// </summary>
    public class MyStudentsController : Controller
    {
        /// <summary>
        /// Передает в представление список студентов и предметов.
        /// </summary>
        /// <returns>
        /// Представление, позволяющее добавить новое задание для студента.
        /// </returns>
        [HttpGet]
        [Authorize(Roles = "teacher")]
        public ActionResult NewTask()
        {
            // Контекст базы данных.
            ApplicationDbContext dataBase = new ApplicationDbContext();

            // Получить список всех пользователей.
            IEnumerable<SelectListItem> students = dataBase.Users.Select(s => new SelectListItem { Text = s.LastName + " " + s.FirstName, Value = s.Id });

            // Отфильтровать список пользователей, оставив только студентов.
            ApplicationUserManager userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            students = students.Where(s => userManager.GetRoles(s.Value).Contains("student"));

            // Получить список всех групп.
            IEnumerable<SelectListItem> disciplines = dataBase.Disciplines.Select(s => new SelectListItem { Text = s.Name, Value = s.DisciplineId.ToString() });

            // Передать в представление список студентов.
            this.ViewData["AllStudents"] = students;

            // Передать в представление список предметов.
            this.ViewData["AllDisciplines"] = disciplines;

            return this.View();
        }

        /// <summary>
        /// Передает новое задание в базу данных.
        /// </summary>
        /// <param name="task">
        /// Передаваемое задание.
        /// </param>
        /// <returns>
        /// Перенаправление на представление со списком студентов и предметов.
        /// </returns>
        [HttpPost]
        [Authorize(Roles = "teacher")]
        public ActionResult NewTask(StudentTask task)
        {
            // Контекст базы данных.
            ApplicationDbContext dataBase = new ApplicationDbContext();

            // Указать, что новых решений нет и количество баллов равно нулю.
            task.NewSolutionIsExist = false;
            task.Points = 0;

            // Сохранить изменения.
            dataBase.StudentTasks.Add(task);
            dataBase.SaveChanges();

            return this.RedirectToAction("TasksList");
        }

        /// <summary>
        /// Возвращает форму редактирования задания.
        /// </summary>
        /// <param name="taskId">
        /// Идентификатор задания в базе данных.
        /// </param>
        /// <returns>
        /// Форма редактирования задания.
        /// </returns>
        [HttpGet]
        [Authorize(Roles = "teacher")]
        public ActionResult EditTask(int taskId)
        {
            // Контекст базы данных.
            ApplicationDbContext dataBase = new ApplicationDbContext();

            //Получить задание.
            StudentTask task = dataBase.StudentTasks.First(p => p.StudentTaskId == taskId);

            // Получить список всех пользователей.
            IEnumerable<SelectListItem> students = dataBase.Users.Select(s => new SelectListItem { Text = s.LastName + " " + s.FirstName, Value = s.Id });

            // Отфильтровать список пользователей, оставив только студентов.
            ApplicationUserManager userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            students = students.Where(s => userManager.GetRoles(s.Value).Contains("student"));

            // Получить список всех групп.
            IEnumerable<SelectListItem> disciplines = dataBase.Disciplines.Select(s => new SelectListItem { Text = s.Name, Value = s.DisciplineId.ToString() });

            // Передать в представление список студентов.
            this.ViewData["AllStudents"] = students;

            // Передать в представление список предметов.
            this.ViewData["AllDisciplines"] = disciplines;

            return this.View(task);
        }

        /// <summary>
        /// Изменяет информацию о задании.
        /// </summary>
        /// <param name="task">
        /// Объект, содержащий информацию о редактируемом задании.
        /// </param>
        /// <returns>
        /// Представление со списком задании.
        /// </returns>
        [HttpPost]
        [Authorize(Roles = "teacher")]
        public ActionResult EditTask(StudentTask task)
        {
            // Контекст базы данных.
            ApplicationDbContext dataBase = new ApplicationDbContext();

            // Получить задание.
            StudentTask _task = dataBase.StudentTasks.First(p => p.StudentTaskId == task.StudentTaskId);

            // Изменить информацию о задании.
            _task.UserId = task.UserId;
            _task.DisciplineId = task.DisciplineId;
            _task.Title = task.Title;
            _task.Description = task.Description;
            _task.MaxPoints = task.MaxPoints;

            // Если баллов больше, чем их должно быть, то приравнять их количество максимально возможному.
            if (_task.Points > _task.MaxPoints)
            {
                _task.Points = _task.MaxPoints;
            }

            // Сохранить изменения.
            dataBase.Entry(_task).State = EntityState.Modified;
            dataBase.SaveChanges();

            return this.RedirectToAction("TasksList");
        }

        /// <summary>
        /// Удаляет задание.
        /// </summary>
        /// <param name="taskId">
        /// Идентификатор задания.
        /// </param>
        /// <returns>
        /// Представление со списком задании.
        /// </returns>
        [HttpPost]
        [Authorize(Roles = "teacher")]
        public ActionResult DeleteTask(int taskId)
        {

            string path = Server.MapPath("~/Solutions/");

            // Контекст базы данных.
            ApplicationDbContext dataBase = new ApplicationDbContext();

            StudentTask task = dataBase.StudentTasks.Include(p => p.User).Include(p => p.Discipline).Include(p => p.Solutions).First(p => p.StudentTaskId == taskId);

            // Каскадное удаление информации о решениях задания.
            IQueryable<Solution> solutions = dataBase.Solutions.Include(p => p.StudentTasks).Where(p => p.StudentTaskId == taskId);

            foreach (var item in solutions)
            {
                // Удалить файлы решений.
                if (System.IO.File.Exists(path + item.Path))
                {
                    System.IO.File.Delete(path + item.Path);
                }

                // Удалить запись о решении.
                dataBase.Entry(item).State = EntityState.Deleted;
            }

            // Удалить задание.
            dataBase.Entry(task).State = EntityState.Deleted;
            dataBase.SaveChanges();

            return this.RedirectToAction("TasksList");
        }

        /// <summary>
        /// Передает в представление список студентов и заданий, выданных им.
        /// </summary>
        /// <param name="userId">
        /// Идентификатор студента в базе данных.
        /// </param>
        /// <param name="disciplineId">
        /// Идентификатор учебной дисциплины в базе данных.
        /// </param>
        /// <returns>
        /// Представление со списком заданий всех студентов.
        /// </returns>
        [Authorize(Roles = "teacher")]
        public ActionResult TasksList(string userId, int? disciplineId)
        {
            // Контекст базы данных.
            ApplicationDbContext dataBase = new ApplicationDbContext();

            // Получить данные из таблиц базы данных: StudentTasks, Disciplines и AspNetUsers.
            IQueryable<StudentTask> studentTasks = dataBase.StudentTasks.Include(p => p.Discipline).Include(p => p.User).Include(p => p.Solutions);

            // Отфильтровать задания по выбранной дисциплине, если та указана.
            if (disciplineId != null && disciplineId != 0)
            {
                studentTasks = studentTasks.Where(p => p.DisciplineId == disciplineId);
            }

            // Отфильтровать задания по id студента, если тот указан.
            if (userId != null && userId != "AllStudents")
            {
                studentTasks = studentTasks.Where(p => p.UserId == userId);
            }

            // Сохранить список дисциплин для выпадающего списка.
            List<Discipline> disciplines = dataBase.Disciplines.ToList();

            // Вставить значение "все дисциплины" для выпадающего списка.
            disciplines.Insert(0, new Discipline { Name = "Все дисциплины", DisciplineId = 0 });

            // Сформировать список пользователей в формате "группа имя фамилия" для выпадающего списка.
            var students = dataBase.Users.Select(s => new { Id = s.Id, Name = s.Group.Name + " " + s.LastName + " " + s.FirstName }).OrderBy(s => s.Name).ToList();

            // Отфильтровать пользователей и сохранить в списке только студентов.
            ApplicationUserManager userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            students = students.Where(s => userManager.GetRoles(s.Id).Contains("student")).ToList();

            // Вставить значение "все студенты для выпадающего списка.
            students.Insert(0, new { Id = "AllStudents", Name = "Все студенты" });

            // Модель структуры данных, передаваемая в представление.
            AllStudentsTasksListViewModel allStudentsTasksListViewModel = new AllStudentsTasksListViewModel
            {
                StudentTasks = studentTasks.ToList(),
                Disciplines = new SelectList(disciplines, "DisciplineId", "Name"),
                Students = new SelectList(students, "Id", "Name")
            };

            return this.View(allStudentsTasksListViewModel);
        }

        /// <summary>
        /// Передает в представление информацию о решении задания.
        /// </summary>
        /// <param name="taskId">
        /// Идентификатор решенного задания.
        /// </param>
        /// <returns>
        /// Представление с информацией о решенном задании.
        /// </returns>
        [HttpGet]
        [Authorize(Roles = "teacher")]
        public ActionResult Solution(int taskId)
        {
            // Контекст базы данных.
            ApplicationDbContext dataBase = new ApplicationDbContext();

            // Получить решение.
            Solution solution = dataBase.Solutions.Where(p => p.StudentTaskId == taskId).OrderByDescending(p => p.StudentTaskId).First();

            //Получить задание.
            StudentTask task = dataBase.StudentTasks.First(p => p.StudentTaskId == taskId);

            // Лист баллов для дропбокса в представлении.
            List<SelectListItem> points = new List<SelectListItem>();
            for (int i = 0; i <= task.MaxPoints; i++)
            {
                points.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
            }

            SelectList pointsList = new SelectList(points, "Value", "Text");

            ViewBag.SolutionPath = solution.Path;
            ViewBag.PointsList = pointsList;

            // Указать, что новых решений нет.
            task.NewSolutionIsExist = false;

            // Сохранить изменения.
            dataBase.Entry(task).State = EntityState.Modified;
            dataBase.SaveChanges();

            return this.View(task);
        }

        /// <summary>
        /// Загружает решение задания на компьютер преподавателя.
        /// </summary>
        /// <param name="solutionPath">
        /// Путь к решению на сервере.
        /// </param>
        /// <returns>
        /// Файл с решением задания.
        /// </returns>
        [Authorize(Roles = "teacher")]
        public FilePathResult DownloadSolution(string solutionPath)
        {
            string filePath = Server.MapPath("~/Solutions/" + solutionPath);
            string file_name = string.Format("{0}{1}", "solution", Path.GetExtension(filePath));
            return File(filePath, MediaTypeNames.Application.Octet, file_name);
        }

        /// <summary>
        /// Сохраняет оценку задания.
        /// </summary>
        /// <param name="task">
        /// Объект задания.
        /// </param>
        /// <returns>
        /// Представление с информацией о решенном задания.
        /// </returns>
        [HttpPost]
        [Authorize(Roles = "teacher")]
        public ActionResult EvalateSoluition(StudentTask task)
        {
            // Контекст базы данных.
            ApplicationDbContext dataBase = new ApplicationDbContext();

            // Получить задание и указать количество баллов.
            StudentTask _task = dataBase.StudentTasks.First(p => p.StudentTaskId == task.StudentTaskId);
            _task.Points = task.Points;

            // Сохранить изменения.
            dataBase.Entry(_task).State = EntityState.Modified;
            dataBase.SaveChanges();

            return this.RedirectToAction("Solution", new { taskId = task.StudentTaskId });
        }
    }
}
