using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using ClassLibrary.Core.Models;

namespace ClassLibrary.Core.Service
{
    public class MyStudentsService
    {
        static ApplicationDbContext _dataBase = new ApplicationDbContext();
        static ApplicationUserManager userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();

        // Функция, возвращающая список всех студентов.
        //
        public static IEnumerable<SelectListItem> GetAllStudents()
        {
            // Получить список всех пользователей.
            IEnumerable<SelectListItem> students = _dataBase.Users.Select(s => new SelectListItem { Text = s.LastName + " " + s.FirstName, Value = s.Id });

            // Отфильтровать список пользователей, оставив только студентов.
            //
            ApplicationUserManager userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            students = students.Where(s => userManager.GetRoles(s.Value).Contains("student"));

            return students;
        }

        // Функция, возвращающая список всех групп.
        //
        public static IEnumerable<SelectListItem> GetAllGroups()
        {
            // Получить список всех групп.
            IEnumerable<SelectListItem> disciplines = _dataBase.Disciplines.Select(s => new SelectListItem { Text = s.Name, Value = s.DisciplineId.ToString() });

            return disciplines;
        }

        // Функция, добавляющая задание в базу данных.
        //
        public static void AddTask(StudentTask task)
        {
            _dataBase.StudentTasks.Add(task);

            _dataBase.SaveChanges();
        }

        // Функция, предназначенная для получения списка всех заданий, выданных каждому студенту
        //
        public static AllStudentsTasksListViewModel GetStudentsTasksList(string userId, int? disciplineId)
        {
            // Получить данные из таблиц базы данных: StudentTasks, Disciplines и AspNetUsers.
            IQueryable<StudentTask> studentTasks = _dataBase.StudentTasks.Include("Discipline").Include("User");

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
            List<Discipline> disciplines = _dataBase.Disciplines.ToList();

            // Вставить значение "все дисциплины" для выпадающего списка.
            disciplines.Insert(0, new Discipline { Name = "Все дисциплины", DisciplineId = 0 });

            // Сформировать список пользователей в формате "имя фамилия" для выпадающего списка.
            var students = _dataBase.Users.Select(s => new { Id = s.Id, Name = s.LastName + " " + s.FirstName }).ToList();

            // Отфильтровать пользователей и сохранить в списке только студентов.
            //
            ApplicationUserManager userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
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

            return allStudentsTasksListViewModel;
        }
    }
}
