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
    using System.Data.Entity;
    using System.IO;

    /// <summary>
    /// Предоставляет методы для работы с данными о студентах и выполненных ими заданиях.
    /// </summary>
    public class MyStudentsService
    {
        /// <summary>
        /// Возвращает список всех студентов.
        /// </summary>
        /// <returns>
        /// Список студентов.
        /// </returns>
        public static IEnumerable<SelectListItem> GetAllStudents()
        {
            // Контекст базы данных.
            ApplicationDbContext dataBase = new ApplicationDbContext();

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
            // Контекст базы данных.
            ApplicationDbContext dataBase = new ApplicationDbContext();

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
            // Контекст базы данных.
            ApplicationDbContext dataBase = new ApplicationDbContext();

            // Указать, что новых решений нет и количество баллов равно нулю.
            task.NewSolutionIsExist = false;
            task.Points = 0;

            // Сохранить изменения.
            dataBase.StudentTasks.Add(task);
            dataBase.SaveChanges();
        }

        /// <summary>
        /// Обновляет информацию о задании в базе данных.
        /// </summary>
        /// <param name="task">
        /// Объект с обновленной информацией о задании.
        /// </param>
        public static void UpdateTask(StudentTask tempTask)
        {
            // Контекст базы данных.
            ApplicationDbContext dataBase = new ApplicationDbContext();

            // Получить задание.
            StudentTask task = dataBase.StudentTasks.First(p => p.StudentTaskId == tempTask.StudentTaskId);

            // Изменить информацию о задании.
            task.UserId = tempTask.UserId;
            task.DisciplineId = tempTask.DisciplineId;
            task.Title = tempTask.Title;
            task.Description = tempTask.Description;
            task.MaxPoints = tempTask.MaxPoints;

            // Если баллов больше, чем их должно быть, то приравнять их количество максимально возможному.
            if(task.Points > task.MaxPoints)
            {
                task.Points = task.MaxPoints;
            }

            // Сохранить изменения.
            dataBase.Entry(task).State = EntityState.Modified;
            dataBase.SaveChanges();
        }

        /// <summary>
        /// Удаляет информацию о задании из базы данных.
        /// </summary>
        /// <param name="taskId">
        /// Идентификатор задания.
        /// </param>
        /// <param name="pathToSolution">
        /// Путь к папке Solution на сервере.
        /// </param>
        public static void RemoveTask(int taskId, string pathToSolution)
        {
            // Контекст базы данных.
            ApplicationDbContext dataBase = new ApplicationDbContext();

            StudentTask task = dataBase.StudentTasks.Include(p => p.User).Include(p => p.Discipline).Include(p => p.Solutions).First(p => p.StudentTaskId == taskId);

            // Каскадное удаление информации о решениях задания.
            IQueryable<Solution> solutions = dataBase.Solutions.Include(p => p.StudentTasks).Where(p => p.StudentTaskId == taskId);

            foreach(var item in solutions)
            {
                // Удалить файлы решений.
                if (File.Exists(pathToSolution + item.Path))
                {
                    File.Delete(pathToSolution + item.Path);
                }

                // Удалить запись о решении.
                dataBase.Entry(item).State = EntityState.Deleted;
            }

            // Удалить задание.
            dataBase.Entry(task).State = EntityState.Deleted;
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

        /// <summary>
        /// Возвращает решение задания.
        /// </summary>
        /// <param name="taskId">
        /// Идентификатор задания.
        /// </param>
        /// <returns>
        /// Информация о решении задания.
        /// </returns>
        public static Solution GetSolution(int taskId)
        {
            // Контекст базы данных.
            ApplicationDbContext dataBase = new ApplicationDbContext();

            // Получить решение.
            Solution solution = dataBase.Solutions.Where(p => p.StudentTaskId == taskId).OrderByDescending(p => p.StudentTaskId).First();

            return solution;
        }

        /// <summary>
        /// Возвращает информацию о задании по его id.
        /// </summary>
        /// <param name="taskId">
        /// Идентификатор задания.
        /// </param>
        /// <returns>
        /// Информация о задании.
        /// </returns>
        public static StudentTask GetTask(int taskId)
        {
            // Контекст базы данных.
            ApplicationDbContext dataBase = new ApplicationDbContext();

            //Получить задание.
            StudentTask task = dataBase.StudentTasks.First(p => p.StudentTaskId == taskId);

            return task;
        }

        /// <summary>
        /// Записывает информацию об отсутствии нового решения задания в базу данных.
        /// </summary>
        /// <param name="taskId">
        /// Идентификатор задания.
        /// </param>
        public static void UpdateOfStateTask(int taskId)
        {
            // Контекст базы данных.
            ApplicationDbContext dataBase = new ApplicationDbContext();

            // Получить задание и указать, что новых решений нет.
            StudentTask task = dataBase.StudentTasks.First(p => p.StudentTaskId == taskId);
            task.NewSolutionIsExist = false;

            // Сохранить изменения.
            dataBase.Entry(task).State = EntityState.Modified;
            dataBase.SaveChanges();
        }

        /// <summary>
        /// Обновить значение оценки задания в базе данных.
        /// </summary>
        /// <param name="task">
        /// Объект задания с измененной оценкой.
        /// </param>
        public static void UpdateOfEvaluateSolution(StudentTask tempTask)
        {
            // Контекст базы данных.
            ApplicationDbContext dataBase = new ApplicationDbContext();

            // Получить задание и указать количество баллов.
            StudentTask task = dataBase.StudentTasks.First(p => p.StudentTaskId == tempTask.StudentTaskId);
            task.Points = tempTask.Points;

            // Сохранить изменения.
            dataBase.Entry(task).State = EntityState.Modified;
            dataBase.SaveChanges();
        }
    }
}
