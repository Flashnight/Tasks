using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Core.Models
{
    public class StudentTask
    {
        [Key]
        public int StudentTaskId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string UserId { get; set; }
        public int DisciplineId { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual Discipline Discipline { get; set; }

        public virtual List<Solution> Solutions { get; set; }
    }
}
