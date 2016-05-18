//-----------------------------------------------------------------------
// <copyright file = "MyTasksService.cs" company="OmSTU">
// Copyright (c) OmSTU. All rights reserved.
// </copyright>
// <author> Рудгальский Михаил </author>
// <author> Федоров Виталий </author>
// <author> Денисов Олег </author>
//-----------------------------------------------------------------------

namespace ClassLibrary.Core.Service
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using ClassLibrary.Core.Models;
    using System.Data.Entity;

    /// <summary>
    /// Предоставляет методы для работы с данными о собственных заданиях студента.
    /// </summary>
    public static class MyTasksService
    {
        /// <summary>
        /// Контекст базы данных
        /// </summary>
        private static ApplicationDbContext dataBase = new ApplicationDbContext();

        /// <summary>
        /// Возвращает список заданий студента.
        /// </summary>
        /// <param name="disciplineId">
        /// Идентификатор дисциплины в базе данных.
        /// </param>
        /// <param name="currentUserId">
        /// Идентификатор текущего пользователя (студента) в базе данных.
        /// </param>
        /// <returns>
        /// Представление, отображающее фильтруемый список заданий текущего студента.
        /// </returns>
        public static MyTasksListViewModel GetMyTaskList(int? disciplineId, string currentUserId)
        {
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

            return myTasksListViewModel;
        }

        /// <summary>
        /// Возвращает описание задания.
        /// </summary>
        /// <param name="taskId">
        /// Идентификатор задания в базе данных.
        /// </param>
        /// <returns>
        /// Данные о задании.
        /// </returns>
        public static StudentTask GetMyTask(int taskId)
        {
            // Найти задачу по id в базе данных.
            StudentTask task = dataBase.StudentTasks.FirstOrDefault<StudentTask>(p => p.StudentTaskId == taskId);

            return task;
        }

        /// <summary>
        /// Сохраняет файл с решением задания в файловую систему,
        /// задаёт ему случайное имя и сохраняет имя файла в базу данных.
        /// </summary>
        /// <param name="taskId">
        /// Идентификатор задания.
        /// </param>
        /// <param name="upload">
        /// Данные о загружаемом файле.
        /// </param>
        /// <param name="path">
        /// Путь к папке с решениями.
        /// </param>
        public static void UploadSolution(int taskId, HttpPostedFileBase upload, string path)
        {
            if (upload != null)
            {
                // Присвоить файлу случайное имя через Guid.
                string fileName = string.Format("{0}{1}", Guid.NewGuid(), Path.GetExtension(upload.FileName));

                // Сохранить файл решения.
                upload.SaveAs(path + fileName);

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
    }
}
