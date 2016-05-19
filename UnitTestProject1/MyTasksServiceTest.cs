//-----------------------------------------------------------------------
// <copyright file = "MyTasksServiceTest.cs" company="OmSTU">
// Copyright (c) OmSTU. All rights reserved.
// </copyright>
// <author> Рудгальский Михаил </author>
// <author> Федоров Виталий </author>
// <author> Денисов Олег </author>
//-----------------------------------------------------------------------

namespace UnitTestProject1
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ClassLibrary.Core.Service;

    /// <summary>
    /// Класс тестирования методов класса MyTasksService
    /// </summary>
    [TestClass]
    public class MyTasksServiceTest
    {
        /// <summary>
        /// Метод тестирования метода GetMyTasks()
        /// </summary>
        [TestMethod]
        public void GetMyTasksTest()
        {
            int taskId = 6;

            MyTasksService.GetMyTask(taskId);
        }

        /// <summary>
        /// Метод тестирования метода GetMyTasksList()
        /// </summary>
        [TestMethod]
        public void GetMyTasksListTest()
        {
            string currentUserId = "dc414eb3-6be5-47da-a31c-fa575f64a17f";
            int disciplineId = 1;

            MyTasksService.GetMyTaskList(disciplineId, currentUserId);
        }
    }
}
