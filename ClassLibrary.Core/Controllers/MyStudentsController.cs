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
    using System.Web.Mvc;
    using ClassLibrary.Core.Models;
    using ClassLibrary.Core.Service;
    using System.IO;
    using System.Net.Mime;

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
            // Передать в представление список студентов.
            this.ViewData["AllStudents"] = MyStudentsService.GetAllStudents();

            // Передать в представление список предметов.
            this.ViewData["AllDisciplines"] = MyStudentsService.GetAllGroups();

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
            MyStudentsService.AddTask(task);

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
            // Получить данные обо всех заданиях, выданных каждому студенту.
            AllStudentsTasksListViewModel allStudentsTasksListViewModel = MyStudentsService.GetStudentsTasksList(userId, disciplineId);

            return this.View(allStudentsTasksListViewModel);
        }

        /// <summary>
        /// Передает в представление информацию о решении задания.
        /// </summary>
        /// <param name="taskId">
        /// Идентификатор решенного задания.
        /// </param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "teacher")]
        public ActionResult Solution(int taskId)
        {
            ApplicationUser student = MyStudentsService.GetStudent(taskId);
            Solution solution = MyStudentsService.GetSolution(taskId);
            StudentTask task = MyStudentsService.GetTask(taskId);

            ViewBag.StudentsName = student.LastName + " " + student.LastName + " " + student.Patronymic;
            ViewBag.StudentsGroup = student.Group.GroupId;
            ViewBag.SolutionPath = solution.Path;
            ViewBag.TaskTitle = task.Title;
            ViewBag.TaskDescription = task.Description;

            MyStudentsService.UpdateOfStateTask(taskId);

            return this.View();
        }

        public FilePathResult DownloadSolution(string solutionPath)
        {
            string filePath = Server.MapPath("~/Solutions/" + solutionPath);
            string file_name = string.Format("{0}{1}", "solution", Path.GetExtension(filePath));
            return File(filePath, MediaTypeNames.Application.Octet, file_name);
        }
    }
}
