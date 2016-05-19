//-----------------------------------------------------------------------
// <copyright file = "MyStudentsServiceTest.cs" company="OmSTU">
// Copyright (c) OmSTU. All rights reserved.
// </copyright>
// <author> Рудгальский Михаил </author>
// <author> Федоров Виталий </author>
// <author> Денисов Олег </author>
//-----------------------------------------------------------------------

namespace UnitTestProject1
{
    using System;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ClassLibrary.Core.Models;
    using ClassLibrary.Core.Service;

    /// <summary>
    /// Класс тестирования класса MyStudentsService.
    /// </summary>
    [TestClass]
    public class MyStudentsServiceTest
    {
        /// <summary>
        /// Тестовый идентификатор пользователя
        /// </summary>
        private string userId = "058e9ca6-36ee-4523-b1d3-713351cc212a";

        /// <summary>
        /// Тестовый идентификатор дисциплины.
        /// </summary>
        private int disciplineId = 1;

        /// <summary>
        /// Метод тестирования метода NewTask().
        /// </summary>
        [TestMethod]
        public void AddTaskTest()
        {
            StudentTask task = new StudentTask { Title = "Пример", Description = "Пример", UserId = userId, DisciplineId = disciplineId, Points = 0, MaxPoints = 100, NewSolutionIsExist = false };

            MyStudentsService.AddTask(task);
        }
    }
}