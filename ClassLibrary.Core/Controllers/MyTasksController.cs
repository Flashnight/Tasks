//-----------------------------------------------------------------------
// <copyright file = "MyTasksController.cs" company="OmSTU">
// Copyright (c) OmSTU. All rights reserved.
// </copyright>
// <author> Рудгальский Михаил </author>
// <author> Федоров Виталий </author>
// <author> Денисов Олег </author>
//-----------------------------------------------------------------------

namespace ClassLibrary.Core.Controllers
{
    using System.Web;
    using System.Web.Mvc;
    using ClassLibrary.Core.Models;
    using ClassLibrary.Core.Service;
    using Microsoft.AspNet.Identity;

    /// <summary>
    /// Предоставляет контроллер с методами для работы с данными о собственных заданиях студента.
    /// </summary>
    public class MyTasksController : Controller
    {
        /// <summary>
        /// Передает список заданий студента в представление.
        /// </summary>
        /// <param name="disciplineId">
        /// Идентификатор учебной дисциплины в базе данных.
        /// </param>
        /// <returns>
        /// Прдеставление, отображающее собственный список заданий студента.
        /// </returns>
        [Authorize(Roles = "student")]
        public ActionResult List(int? disciplineId)
        {
            // Id текущего пользователя.
            string currentUserId = User.Identity.GetUserId();

            // Получить список заданий из базы данных.
            MyTasksListViewModel myTasksListViewModel = MyTasksService.GetMyTaskList(disciplineId, currentUserId);

            return this.View(myTasksListViewModel);
        }

        /// <summary>
        /// Передает информацию о конкретном задании в представление.
        /// </summary>
        /// <param name="taskId">
        /// Идентификатор задания в базе данных.
        /// </param>
        /// <returns>
        /// Представление с описанием конкретного задания студента.
        /// </returns>
        [Authorize(Roles = "student")]
        public ActionResult Description(int taskId)
        {
            // Получить информацию о задании из бд.
            StudentTask task = MyTasksService.GetMyTask(taskId);

            // Передать задания в представление.
            ViewBag.Task = task;    

            return this.View();
        }

        /// <summary>
        /// Загружает файл решения задания в папку Solutions данного проекта,
        /// задает ему случайное имя и сохраняет имя файла в базу данных.
        /// </summary>
        /// <param name="taskId">
        /// Идентификатор задания в базе данных.
        /// </param>
        /// <param name="upload">
        /// Данные о загружаемом файле.
        /// </param>
        /// <returns>
        /// Представление, отображающее список заданий.
        /// </returns>
        [Authorize(Roles = "student")]
        [HttpPost]
        public ActionResult UploadSolution(int taskId, HttpPostedFileBase upload)
        {
            // Если указан файл, то загрузить его на сервер.
            if (upload != null)
            {
                // Получить физический путь к папке с решениями.
                string path = Server.MapPath("~/Solutions/");

                // Сохранить файл решения в файловой системе.
                MyTasksService.UploadSolution(taskId, upload, path);
            }

            // Перенаправить на список заданий.
            return this.RedirectToAction("List");
        }
    }
}