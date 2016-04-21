//-----------------------------------------------------------------------
// <copyright file = "IdentityModel.cs" company="OmSTU">
// Copyright (c) OmSTU. All rights reserved.
// </copyright>
// <author> Рудгальский Михаил </author>
// <author> Федоров Виталий </author>
// <author> Денисов Олег </author>
//----------------------------------------------------------------------

namespace ClassLibrary.Core.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Entity;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;

    /// <summary>
    /// Предоставляет модель данных о пользователях, наследуемую от IdentityUser.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        // Чтобы добавить данные профиля для пользователя, можно добавить дополнительные свойства в класс ApplicationUser. Дополнительные сведения см. по адресу: http://go.microsoft.com/fwlink/?LinkID=317594.

        /// <summary>
        /// Фамилия.
        /// </summary>
        [Required]
        public string LastName { get; set; }

        /// <summary>
        /// Имя.
        /// </summary>
        [Required]
        public string FirstName { get; set; }
        /// <summary>
        /// Отчество.
        /// </summary>
        public string Patronymic { get; set; }

        /// <summary>
        /// Студенческая группа, если пользователь является студентом.
        /// </summary>
        public int? GroupId { get; set; }

        /// <summary>
        /// Подгруппа, если пользователь является студентом.
        /// </summary>
        public int? Subgroup { get; set; }

        /// <summary>
        /// Ссылка на запись в базе данных о студенческой группе, в которой состоит пользователь, если он является студентом.
        /// </summary>
        public virtual Group Group { get; set; }

        /// <summary>
        /// Список всех заданий пользователя, если он является студентом.
        /// </summary>
        public virtual List<StudentTask> StudentTasks { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Обратите внимание, что authenticationType должен совпадать с типом, определенным в CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Здесь добавьте утверждения пользователя
            return userIdentity;
        }
    }

    /// <summary>
    /// Контекст базы данных для создания таблиц, если база данных не создана.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        /// <summary>
        /// Настраивает подключение к базе данных (строка подключения представлена в App.Config).
        /// </summary>
        public ApplicationDbContext()
            : base("Tasks", throwIfV1Schema: false)
        {
        }

        /// <summary>
        /// Коллекция данных о студенческих группах.
        /// </summary>
        public DbSet<Group> Groups { get; set; }

        /// <summary>
        /// Коллекция данных об учебных дисциплинах.
        /// </summary>
        public DbSet<Discipline> Disciplines { get; set; }

        /// <summary>
        /// Коллекция данных о заданиях студентов.
        /// </summary>
        public DbSet<StudentTask> StudentTasks { get; set; }

        /// <summary>
        /// Коллекция данных о решениях заданий.
        /// </summary>
        public DbSet<Solution> Solutions { get; set; }

        /// <summary>
        /// Созданать базу данных.
        /// </summary>
        /// <returns>
        /// Контекст базы данных.
        /// </returns>
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}