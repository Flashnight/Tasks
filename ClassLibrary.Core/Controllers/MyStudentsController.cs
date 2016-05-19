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
    using System.Collections.Generic;

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
            StudentTask task = MyStudentsService.GetTask(taskId);

            // Передать в представление список студентов.
            this.ViewData["AllStudents"] = MyStudentsService.GetAllStudents();

            // Передать в представление список предметов.
            this.ViewData["AllDisciplines"] = MyStudentsService.GetAllGroups();

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
            MyStudentsService.UpdateTask(task);

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

            MyStudentsService.RemoveTask(taskId, path);

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
        /// <returns>
        /// Представление с информацией о решенном задании.
        /// </returns>
        [HttpGet]
        [Authorize(Roles = "teacher")]
        public ActionResult Solution(int taskId)
        {
            Solution solution = MyStudentsService.GetSolution(taskId);
            StudentTask task = MyStudentsService.GetTask(taskId);

            // Лист баллов для дропбокса в представлении.
            List<SelectListItem> points = new List<SelectListItem>();
            for (int i = 0; i <= task.MaxPoints; i++)
            {
                points.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
            }

            SelectList pointsList = new SelectList(points, "Value", "Text");

            ViewBag.SolutionPath = solution.Path;
            ViewBag.PointsList = pointsList;

            MyStudentsService.UpdateOfStateTask(taskId);

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
            MyStudentsService.UpdateOfEvaluateSolution(task);

            return this.RedirectToAction("Solution", new { taskId = task.StudentTaskId });
        }
    }
}
