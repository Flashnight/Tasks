//-----------------------------------------------------------------------
// <copyright file = "Discipline.cs" company="OmSTU">
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

    /// <summary>
    /// Предоставляет модель данных об учебной дисциплине.
    /// </summary>
    public class Discipline
    {
        /// <summary>
        /// Идентификатор учебной дисциплины в базе данных.
        /// </summary>
        [Key]
        public int DisciplineId { get; set; }

        /// <summary>
        /// Наименование учебной дисциплины.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Список всех заданий для всех студентов по данной учебной дисциплине.
        /// </summary>
        public virtual List<StudentTask> StudentTasks { get; set; }
    }
}
