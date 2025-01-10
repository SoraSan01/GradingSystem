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
        public string StudentId { get; set; }

        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }

        [Required]
        public string SubjectId { get; set; }

        [ForeignKey("SubjectId")]
        public virtual Subject Subject { get; set; }

        [Required]
        public string ProgramId { get; set; }

        [ForeignKey("ProgramId")]
        public virtual Program Program { get; set; }

        [Required]
        public decimal Grades { get; set; }

        public DateTime DateSubmitted { get; set; }

        public DateTime CreatedAt { get; set; }

        // Computed property to return the student's name
        public string StudentName
        {
            get { return Student != null ? $"{Student.FirstName} {Student.LastName}" : "Unknown"; }
        }

        public string StudentCourse
        {
            get { return Program != null ? $"{Program.ProgramName}" : "Unknown"; }
        }
        public Grade()
        {
            CreatedAt = DateTime.Now;
            DateSubmitted = DateTime.Now;
        }
    }
}
