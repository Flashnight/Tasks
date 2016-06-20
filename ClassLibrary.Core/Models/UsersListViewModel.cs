//-----------------------------------------------------------------------
// <copyright file = "UserListViewModel.cs" company="OmSTU">
// Copyright (c) OmSTU. All rights reserved.
// </copyright>
// <author> Рудгальский Михаил </author>
// <author> Федоров Виталий </author>
// <author> Денисов Олег </author>
//----------------------------------------------------------------------

namespace ClassLibrary.Core.Models
{
    using System.Collections.Generic;
    using System.Web.Mvc;

    /// <summary>
    /// Предоставляет модель данных для фильтрации списка пользователей по ролям, а также фильтрации студентов по группам.
    /// </summary>
    public class UsersListViewModel
    {
        /// <summary>
        /// Список всех студентов.
        /// </summary>
        public IEnumerable<ApplicationUser> Users { get; set; }
        /// <summary>
        /// Список всех ролей пользователей.
        /// </summary>
        public SelectList Roles { get; set; }
        /// <summary>
        /// Список всех групп, в которых состоят студенты.
        /// </summary>
        public SelectList Groups { get; set; }
    }
}
