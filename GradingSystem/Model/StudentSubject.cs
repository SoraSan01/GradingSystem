using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GradingSystem.Model
{
    [Table("StudentSubjects")]
    public class StudentSubject
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string StudentId { get; set; }

        // Define the relationship with Student entity
        [ForeignKey(nameof(StudentId))]
        public Student Student { get; set; }

        [Required]
        public string SubjectId { get; set; }

        // Define the relationship with Subject entity
        [ForeignKey(nameof(SubjectId))]
        public Subject Subject { get; set; }

        // Changed grades to decimal (optional based on requirements)
        public decimal? Grade { get; set; }  // Nullable if grades can be empty

        public DateTime CreatedAt { get; set; }

        // This ensures the CourseCode is not saved to the database
        [NotMapped]
        public string CourseCode => Subject?.CourseCode;

        // These are the additional properties you need
        [NotMapped]
        public string Professor => Subject?.ProfessorName;

        [NotMapped]
        public string Schedule => Subject?.Schedule;

        [NotMapped]
        public bool IsGradeLow => Grade < 75;

        // Constructor to set CreatedAt
        public StudentSubject()
        {
            CreatedAt = DateTime.Now;
        }
    }
}
