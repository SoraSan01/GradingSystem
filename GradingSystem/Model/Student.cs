using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GradingSystem.Model
{
    [Table("Students")]

    public class Student
    {
        [Key]
        public string StudentId { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(50)]
        public string Program { get; set; }

        [Required]
        public string YearLevel { get; set; }
        public DateTime CreatedAt { get; set; }


        public Student()
        {
            CreatedAt = DateTime.Now;
        }


    }
}
