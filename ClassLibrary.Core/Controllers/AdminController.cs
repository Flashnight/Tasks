//-----------------------------------------------------------------------
// <copyright file = "AdminController.cs" company="OmSTU">
// Copyright (c) OmSTU. All rights reserved.
// </copyright>
// <author> Рудгальский Михаил </author>
//-----------------------------------------------------------------------

namespace ClassLibrary.Core.Controllers
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using ClassLibrary.Core.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.AspNet.Identity.EntityFramework;

    /// <summary>
    /// Предоставляет контроллер для управления учетными записями пользователей.
    /// </summary>
    public class AdminController : Controller
    {
        /// <summary>
        /// Возвращает список всех пользователей или список пользователей, принадлежащих определенной роли.
        /// </summary>
        /// <param name="role">
        /// Роль пользователей, по которой выполняется фильтрация.
        /// </param>
        /// <returns>
        /// Представление со списком всех пользователей.
        /// </returns>
        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult UsersList(string roleId, int? groupId)
        {
            ApplicationDbContext dataBase = new ApplicationDbContext();

            // Получить список пользователей.
            IEnumerable<ApplicationUser> users = dataBase.Users.Include(p => p.Roles);

            // Получить список всех ролей пользователей.
            List<IdentityRole> roles = dataBase.Roles.ToList();

            // Отфильтровать пользователей по роли.
            if (roleId != null && roleId != "AllRoles")
            {
                ApplicationUserManager userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                // Не знаю, почему, но без .ToList().AsEnumerable() Where() выдаст Null.
                users = users.Where(p => userManager.IsInRole(p.Id, roles.First(t => t.Id == roleId).Name)).ToList().AsEnumerable();
            }

            if (groupId != null && groupId != 0 && roleId == "student")
            {
                users = users.Where(p => p.GroupId == groupId);
            }


            foreach(IdentityRole role in roles)
            {
                switch(role.Name)
                {
                    case "admin":
                        role.Name = "Администратор";
                        break;
                    case "teacher":
                        role.Name = "Преподаватель";
                        break;
                    case "student":
                        role.Name = "Студент";
                        break;
                }
            }

            List<Group> groups = dataBase.Groups.ToList();
            groups.Insert(0, new Group { GroupId = 0, Name = "Все группы" });

            // Вставить элемент в список ролей для вывода пользователей независимо от роли.
            roles.Insert(0, new IdentityRole { Id = "AllRoles", Name = "Все роли" });

            // Модель структуры данных, передаваемой в представление.
            UsersListViewModel usersListViewModel = new UsersListViewModel
            {
                Users = users.ToList(),
                Roles = new SelectList(roles, "Id", "Name"),
                Groups = new SelectList(groups, "GroupId", "Name")
            };

            // Передать данные в представление.
            return this.View(usersListViewModel);
        }
    }
}
