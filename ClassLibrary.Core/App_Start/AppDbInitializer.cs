//-----------------------------------------------------------------------
// <copyright file = "AppDbInitializer.cs" company="OmSTU">
// Copyright (c) OmSTU. All rights reserved.
// </copyright>
// <author> Рудгальский Михаил </author>
// <author> Федоров Виталий </author>
// <author> Денисов Олег </author>
//-----------------------------------------------------------------------

namespace ClassLibrary.Core.App_Start
{
    using System.Data.Entity;
    using ClassLibrary.Core.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;

    /// <summary>
    /// Предоставляет методы предварительной настройки базы данных.
    /// </summary>
    public class AppDbInitializer : CreateDatabaseIfNotExists<ApplicationDbContext>
    {
        /// <summary>
        /// Добавляет начальные данные в базу данных.
        /// </summary>
        /// <param name="context">
        /// Контекст базы данных.
        /// </param>
        protected override void Seed(ApplicationDbContext context)
        {
            // Создание ролей пользователей
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            // Задание ролей администратора, преподавателя и студента
            var roleAdmin = new IdentityRole { Name = "admin" };
            var roleTeacher = new IdentityRole { Name = "teacher" };
            var roleStudent = new IdentityRole { Name = "student" };

            // Сохранение ролей в базе данных
            roleManager.Create(roleAdmin);
            roleManager.Create(roleTeacher);
            roleManager.Create(roleStudent);

            base.Seed(context);
        }
    }
}
