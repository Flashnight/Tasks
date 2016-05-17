//-----------------------------------------------------------------------
// <copyright file = "StudentTask.cs" company="OmSTU">
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
    using System.Web.Mvc;
    /// <summary>
    /// Предоставляет модель данных о задании.
    /// </summary>
    public class StudentTask
    {
        /// <summary>
        /// Идентификатор задания.
        /// </summary>
        [HiddenInput(DisplayValue = false)]
        [Key]
        public int StudentTaskId { get; set; }

        /// <summary>
        /// Наименование задания.
        /// </summary>
        [Required]
        [Display(Name = "Заголовок")]
        public string Title { get; set; }

        /// <summary>
        /// Описание задания.
        /// </summary>
        [Required]
        [Display(Name = "Описание")]
        public string Description { get; set; }

        /// <summary>
        /// Идентификатор студента в базе данных, которому выдано задание.
        /// </summary>
        [Required]
        [Display(Name = "Студент")]
        public string UserId { get; set; }

        /// <summary>
        /// Идентификатор учебной дисциплины в базе данных, по которой выдано данное задание.
        /// </summary>
        [Required]
        [Display(Name="Дисциплина")]
        public int DisciplineId { get; set; }

        /// <summary>
        /// Ссылка на запись в базе данных о студенте, которому выдано задание.
        /// </summary>
        public virtual ApplicationUser User { get; set; }

        /// <summary>
        /// Ссылка на запись в базе данных о дисциплине, по которой выдано задание.
        /// </summary>
        public virtual Discipline Discipline { get; set; }

        /// <summary>
        /// Список всех загруженных решений данного задания.
        /// </summary>
        public virtual List<Solution> Solutions { get; set; }
    }
}
