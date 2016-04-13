using ClassLibrary.Core.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace ClassLibrary.Core.App_Start
{
    public class AppDbInitializer : CreateDatabaseIfNotExists<ApplicationDbContext>
    {
        //Функция добавляет данные по умолчанию в базу данных
        protected override void Seed(ApplicationDbContext context)
        {
            //Создание ролей пользователей
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            //Задание роли администратора, преподавателя и студента
            var roleAdmin = new IdentityRole { Name = "admin" };
            var roleTeacher = new IdentityRole { Name = "teacher" };
            var roleStudent = new IdentityRole { Name = "student" };

            //Сохранение ролей в базе данных
            roleManager.Create(roleAdmin);
            roleManager.Create(roleTeacher);
            roleManager.Create(roleStudent);

            base.Seed(context);
        }
    }
}
