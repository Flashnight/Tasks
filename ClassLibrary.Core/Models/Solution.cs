using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Core.Models
{
    public class Solution
    {
        [Key]
        public int SolutionId { get; set; }
        public string Path { get; set; }
        public int StudentTaskId { get; set; }

        public virtual StudentTask StudentTasks { get; set; }
    }
}
