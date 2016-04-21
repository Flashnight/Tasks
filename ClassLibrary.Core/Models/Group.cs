//-----------------------------------------------------------------------
// <copyright file = "Group.cs" company="OmSTU">
// Copyright (c) OmSTU. All rights reserved.
// </copyright>
// <author> Рудгальский Михаил </author>
// <author> Федоров Виталий </author>
// <author> Денисов Олег </author>
//-----------------------------------------------------------------------

namespace ClassLibrary.Core.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Предоставляет модель данных о студенческой группе.
    /// </summary>
    public class Group
    {
        /// <summary>
        /// Идентификатор группы в базе данных.
        /// </summary>
        [Key]
        public int GroupId { get; set; }

        /// <summary>
        /// Название группы.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Список всех студентов, состоящих в данной группе.
        /// </summary>
        public virtual List<ApplicationUser> Users { get; set; }
    }
}
