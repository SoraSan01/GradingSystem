using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GradingSystem.Model
{
    [Table("Grades")]
    public class Grade
    {
        [Key]
        public string GradeId { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string StudentId { get; set; }

        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }

        [Required]
        public string Program { get; set; }

        [Required]
        public string YearLevel { get; set; }

        [Required]
        public string Semester {  get; set; } 

        public DateTime CreatedAt { get; set; }

        // Computed property to return the student's name
        public string StudentName
        {
            get { return Student != null ? $"{Student.FirstName} {Student.LastName}" : "Unknown"; }
        }

        public Grade()
        {
            CreatedAt = DateTime.Now;
        }
    }
}
