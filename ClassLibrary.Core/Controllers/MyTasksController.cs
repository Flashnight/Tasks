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
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using ClassLibrary.Core.Models;
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

            // Контекст базы данных.
            ApplicationDbContext dataBase = new ApplicationDbContext();

            IQueryable<StudentTask> studentTasks = dataBase.StudentTasks.Include("Discipline");

            // Получить все задачи пользователя.
            studentTasks = studentTasks.Where(p => p.UserId == currentUserId);

            // Отфильтровать задания по выбранной дисциплине, если та указана.
            if (disciplineId != null && disciplineId != 0)
            {
                studentTasks = studentTasks.Where(p => p.DisciplineId == disciplineId);
            }

            // Список дисциплин.
            List<Discipline> disciplines = dataBase.Disciplines.ToList();

            // Выбрать все задания.
            disciplines.Insert(0, new Discipline { Name = "Все дисциплины", DisciplineId = 0 });

            // Данные, которые будут переданы в представление.
            MyTasksListViewModel myTasksListViewModel = new MyTasksListViewModel
            {
                StudentTasks = studentTasks.ToList(),
                Disciplines = new SelectList(disciplines, "DisciplineId", "name")
            };

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
            // Контекст базы данных.
            ApplicationDbContext dataBase = new ApplicationDbContext();

            // Найти задачу по id в базе данных.
            StudentTask task = dataBase.StudentTasks.FirstOrDefault<StudentTask>(p => p.StudentTaskId == taskId);

            return this.View(task);
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

                if (upload != null)
                {
                    // Контекст базы данных.
                    ApplicationDbContext dataBase = new ApplicationDbContext();

                    // Присвоить файлу случайное имя через Guid.
                    string fileName = string.Format("{0}{1}", Guid.NewGuid(), Path.GetExtension(upload.FileName));

                    // Сохранить файл решения.
                    upload.SaveAs(path + fileName);

                    // Удалить все прочие решения.
                    IQueryable<Solution> solutions = dataBase.Solutions.Where(p => p.StudentTaskId == taskId);

                    foreach (var item in solutions)
                    {
                        // Удалить файл решения.
                        if (System.IO.File.Exists(path + item.Path))
                        {
                            System.IO.File.Delete(path + item.Path);
                        }

                        // Удалить запись из базы данных.
                        dataBase.Entry(item).State = EntityState.Deleted;
                    }

                    // Сохранить имя файла в базе данных.
                    Solution newSolution = new Solution { Path = fileName, StudentTaskId = taskId };
                    dataBase.Solutions.Add(newSolution);

                    // Добавить оповещение о новом решении.
                    StudentTask task = dataBase.StudentTasks.Where(p => p.StudentTaskId == taskId).First();
                    task.NewSolutionIsExist = true;
                    dataBase.Entry(task).State = EntityState.Modified;

                    dataBase.SaveChanges();
                }
            }

            // Перенаправить на список заданий.
            return this.RedirectToAction("List");
        }
    }
}