using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Policy;
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

        [ForeignKey("Program")]
        public string ProgramId { get; set; }  // The ProgramId links to the Program

        // Navigation property to the Program model
        public Program Program { get; set; }

        [Required]
        [StringLength(100)]
        public string Semester { get; set; }

        [Required]
        public string YearLevel { get; set; }

        [Required]
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public string StudentName => $"{FirstName} {LastName}";

        public Student()
        {
            CreatedAt = DateTime.Now;
        }


    }
}
