//-----------------------------------------------------------------------
// <copyright file = "MyStudentsControllerTest.cs" company="OmSTU">
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
    using ClassLibrary.Core.Controllers;
    using static System.Net.WebRequestMethods;
    /// <summary>
    /// Класс тестирования контроллера MyStudentsController.
    /// </summary>
    [TestClass]
    public class MyStudentsControllerTest
    {
        /// <summary>
        /// Метод тестирования метода NewTask().
        /// Тест провалится, поскольку потребуется авторизация пользователя.
        /// </summary>
        [TestMethod]
        public void NewTaskTest()
        {
            // Создать экземпляр класса контроллера.
            MyStudentsController controller = new MyStudentsController();
            
            // Запустить тест метода.
            ViewResult result = controller.NewTask() as ViewResult;

            // Вывести результат.
            Assert.IsNotNull(result);
        }
    }
}
