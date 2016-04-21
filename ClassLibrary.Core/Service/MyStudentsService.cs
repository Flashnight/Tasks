//-----------------------------------------------------------------------
// <copyright file = "MyStudentsService.cs" company="OmSTU">
// Copyright (c) OmSTU. All rights reserved.
// </copyright>
// <author> Рудгальский Михаил </author>
// <author> Федоров Виталий </author>
// <author> Денисов Олег </author>
//-----------------------------------------------------------------------

namespace ClassLibrary.Core.Service
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using ClassLibrary.Core.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;

    /// <summary>
    /// Предоставляет методы для работы с данными о студентах и выполненных ими заданиях.
    /// </summary>
    public class MyStudentsService
    {
        /// <summary>
        /// Контест базы данных.
        /// </summary>
        private static ApplicationDbContext dataBase = new ApplicationDbContext();

        /// <summary>
        /// Возвращает список всех студентов.
        /// </summary>
        /// <returns>
        /// Список студентов.
        /// </returns>
        public static IEnumerable<SelectListItem> GetAllStudents()
        {
            // Получить список всех пользователей.
            IEnumerable<SelectListItem> students = dataBase.Users.Select(s => new SelectListItem { Text = s.LastName + " " + s.FirstName, Value = s.Id });

            // Отфильтровать список пользователей, оставив только студентов.
            ApplicationUserManager userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            students = students.Where(s => userManager.GetRoles(s.Value).Contains("student"));

            return students;
        }

        /// <summary>
        /// Возвращает список всех групп, в которых состоят студенты.
        /// </summary>
        /// <returns>
        /// Список всех групп, в которых состоят студенты.
        /// </returns>
        public static IEnumerable<SelectListItem> GetAllGroups()
        {
            // Получить список всех групп.
            IEnumerable<SelectListItem> disciplines = dataBase.Disciplines.Select(s => new SelectListItem { Text = s.Name, Value = s.DisciplineId.ToString() });

            return disciplines;
        }

        /// <summary>
        /// Добавляет новое задание в базу данных.
        /// </summary>
        /// <param name="task">
        /// Данные о добавляемом задании.
        /// </param>
        public static void AddTask(StudentTask task)
        {
            dataBase.StudentTasks.Add(task);

            dataBase.SaveChanges();
        }

        /// <summary>
        /// Возвращает список студентов и заданий для каждого из них.
        /// </summary>
        /// <param name="userId">
        /// Идентификатор студента в базе данных.
        /// </param>
        /// <param name="disciplineId">
        /// Идентификатор учебной дисциплины в базе данных.
        /// </param>
        /// <returns>
        /// Список студентов и заданий для каждого из них.
        /// </returns>
        public static AllStudentsTasksListViewModel GetStudentsTasksList(string userId, int? disciplineId)
        {
            // Получить данные из таблиц базы данных: StudentTasks, Disciplines и AspNetUsers.
            IQueryable<StudentTask> studentTasks = dataBase.StudentTasks.Include("Discipline").Include("User");

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

            // Сформировать список пользователей в формате "имя фамилия" для выпадающего списка.
            var students = dataBase.Users.Select(s => new { Id = s.Id, Name = s.LastName + " " + s.FirstName }).ToList();

            // Отфильтровать пользователей и сохранить в списке только студентов.
            ApplicationUserManager userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
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

            return allStudentsTasksListViewModel;
        }
    }
}
