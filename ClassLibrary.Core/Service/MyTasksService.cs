using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web.Mvc;
using ClassLibrary.Core.Models;

namespace ClassLibrary.Core.Service
{
    public static class MyTasksService
    {
        static ApplicationDbContext _dataBase = new ApplicationDbContext();

        // Метод, возвращающий список заданий студента.
        //
        public static MyTasksListViewModel GetMyTaskList(int? disciplineId, string currentUserId)
        {
            IQueryable<StudentTask> studentTasks = _dataBase.StudentTasks.Include("Discipline");

            // получить все задачи пользователя.
            studentTasks = studentTasks.Where(p => p.UserId == currentUserId);

            //отфильтровать задания по выбранной дисциплине, если та указана.
            //
            if (disciplineId != null && disciplineId != 0)
            {
                studentTasks = studentTasks.Where(p => p.DisciplineId == disciplineId); 
            }

            //список дисциплин.
            List<Discipline> disciplines = _dataBase.Disciplines.ToList();

            //выбрать все задания.
            disciplines.Insert(0, new Discipline { Name = "Все дисциплины", DisciplineId = 0 });

            //данные, которые будут переданы в представление.
            //
            MyTasksListViewModel myTasksListViewModel = new MyTasksListViewModel   
            {
                StudentTasks = studentTasks.ToList(),
                Disciplines = new SelectList(disciplines, "DisciplineId", "name")
            };

            return myTasksListViewModel;
        }

        // Метод, возвращающий описание задания.
        //
        public static StudentTask GetMyTask(int taskId)
        {
            //найти задачу по id в базе данных.
            StudentTask task = _dataBase.StudentTasks.FirstOrDefault<StudentTask>(p => p.StudentTaskId == taskId);

            return task;
        }

        // Метод, сохраняющий файл с решение задания в файловую систему
        // и сохраняющий имя файла в базу данных.
        //
        public static void UploadSolution(int taskId, HttpPostedFileBase upload, string path)
        {
            if(upload!=null)
            {
                //Присвоить файлу случайное имя через Guid.
                string fileName = string.Format("{0}.{1}", Guid.NewGuid(), Path.GetExtension(upload.FileName));

                //Сохранить файл решения.
                upload.SaveAs(path + fileName);

                //Сохранить имя файла в базе данных.
                //
                Solution NewSolution = new Solution { Path = fileName, StudentTaskId = taskId };
                _dataBase.Solutions.Add(NewSolution);
                _dataBase.SaveChanges();
            }
        }
    }
}
