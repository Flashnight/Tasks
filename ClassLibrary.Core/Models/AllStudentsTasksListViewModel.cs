using System.Collections.Generic;
using System.Web.Mvc;

namespace ClassLibrary.Core.Models
{
    // Модель данных, используемых для выбора заданий всех студентов.
    //
    public class AllStudentsTasksListViewModel
    {
        // Список заданий всех студентов
        public IEnumerable<StudentTask> StudentTasks { get; set; }
        // Список учебных дисциплин.
        public SelectList Disciplines { get; set; }
        // Список студентов
        public SelectList Students { get; set; }
    }
}
