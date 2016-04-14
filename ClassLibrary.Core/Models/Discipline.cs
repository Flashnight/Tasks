using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Core.Models
{
    public class Discipline
    {
        [Key]
        public int DisciplineId { get; set; }
        [Required]
        public string Name { get; set; }

        public virtual List<StudentTask> StudentTasks { get; set; }
    }
}
