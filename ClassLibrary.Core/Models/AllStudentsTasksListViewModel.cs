//-----------------------------------------------------------------------
// <copyright file = "AllStudentsTasksListViewModel.cs" company="OmSTU">
// Copyright (c) OmSTU. All rights reserved.
// </copyright>
// <author> Рудгальский Михаил </author>
// <author> Федоров Виталий </author>
// <author> Денисов Олег </author>
//-----------------------------------------------------------------------

namespace ClassLibrary.Core.Models
{
    using System.Collections.Generic;
    using System.Web.Mvc;

    /// <summary>
    /// Предоставляет модель данных для фильтрации списка заданий 
    /// </summary>
    public class AllStudentsTasksListViewModel
    {
        /// <summary>
        /// Список всех заданий.
        /// </summary>
        public IEnumerable<StudentTask> StudentTasks { get; set; }

        /// <summary>
        /// Список всех дисциплин.
        /// </summary>
        public SelectList Disciplines { get; set; }

        /// <summary>
        /// Список всех студентов.
        /// </summary>
        public SelectList Students { get; set; }
    }
}
