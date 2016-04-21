//-----------------------------------------------------------------------
// <copyright file = "Solution.cs" company="OmSTU">
// Copyright (c) OmSTU. All rights reserved.
// </copyright>
// <author> Рудгальский Михаил </author>
// <author> Федоров Виталий </author>
// <author> Денисов Олег </author>
//----------------------------------------------------------------------

namespace ClassLibrary.Core.Models
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Предоставляет модель данных решения задания.
    /// </summary>
    public class Solution
    {
        /// <summary>
        /// Идентификатор решения в базе данных.
        /// </summary>
        [Key]
        public int SolutionId { get; set; }

        /// <summary>
        /// Относительный путь к решению.
        /// </summary>
        [Required]
        public string Path { get; set; }

        /// <summary>
        /// Идентификатор задания в базе данных, которому принадлежит решение.
        /// </summary>
        public int StudentTaskId { get; set; }

        /// <summary>
        /// Ссылка на запись в базе данных о задании, которому принадлежит решение.
        /// </summary>
        public virtual StudentTask StudentTasks { get; set; }
    }
}
