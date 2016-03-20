using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ClassLibrary.Core.Models
{
    public class MyTasksListViewModel
    {
        public IEnumerable<StudentTask> StudentTasks { get; set; }
        public SelectList Disciplines { get; set; }
    }
}
