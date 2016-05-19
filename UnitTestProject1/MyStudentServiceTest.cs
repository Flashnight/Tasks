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
        private string userId = "dc414eb3-6be5-47da-a31c-fa575f64a17f";

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
            StudentTask task = new StudentTask {  Title = "Пример", Description = "Пример", UserId = userId, DisciplineId = disciplineId,  MaxPoints = 100 };

            MyStudentsService.AddTask(task);
        }

        /// <summary>
        /// Метод тестирования метода UpdateTaskTest()
        /// </summary>
        [TestMethod]
        public void UpdateTaskTest()
        {
            StudentTask task = new StudentTask { StudentTaskId = 8, Title = "Пример2", Description = "Пример2", UserId = userId, DisciplineId = 2, MaxPoints = 100 };

            MyStudentsService.UpdateTask(task);
        }
        
        /// <summary>
        /// Метод тестирования метода RemoveTask()
        /// </summary>
        [TestMethod]
        public void RemoveTaskTest()
        {
            int taskId = 9;
            string path = @"D:\GitHub\Tasks\Tasks\Solutions";

            MyStudentsService.RemoveTask(taskId, path);
        }
    }
}