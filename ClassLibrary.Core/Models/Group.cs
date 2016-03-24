using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Core.Models
{
    public class Group
    {
        [Key]
        public int GroupId { get; set; }
        public string Name { get; set; }

        public virtual List<ApplicationUser> Users { get; set; }
    }
}
