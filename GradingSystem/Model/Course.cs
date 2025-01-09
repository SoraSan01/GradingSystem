using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GradingSystem.Model
{
    public class Course
    {
        [Key]
        public string CourseId { get; set; }

        [Required]
        [StringLength(50)]
        public string CourseName { get; set; }

        [Required]
        [StringLength(100)]
        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public Course()
        {
            CreatedAt = DateTime.Now;
        }
    }
}
