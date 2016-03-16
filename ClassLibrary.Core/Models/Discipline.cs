using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Core.Models
{
    public class Discipline
    {
        [Key]
        public int DisciplineId { get; set; }
        public string Name { get; set; }

        public virtual List<StudentTask> StudentTasks { get; set; }
    }
}
