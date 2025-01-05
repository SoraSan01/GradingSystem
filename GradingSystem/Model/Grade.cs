using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GradingSystem.Model
{
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
        public string CourseId { get; set; }

        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }

        [Required]
        public decimal Grades { get; set; }

        public DateTime DateSubmitted { get; set; }

        public DateTime CreatedAt { get; set; }

        public Grade()
        {
            CreatedAt = DateTime.Now;
            DateSubmitted = DateTime.Now;
        }
    }
}
