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

        // Grade as a string to support "INC" or numeric values
        public string? Grade { get; set; } // Allow null values

        public DateTime CreatedAt { get; set; }

        // This ensures the CourseCode is not saved to the database
        [NotMapped]
        public string CourseCode => Subject?.CourseCode;

        // Additional properties
        [NotMapped]
        public string Professor => Subject?.ProfessorName;

        [NotMapped]
        public string Schedule => Subject?.Schedule;


        // Convert Grade to a nullable decimal for calculations (returns null if it's "INC")
        [NotMapped]
        public decimal? GradeAsNumber
        {
            get
            {
                if (decimal.TryParse(Grade, out decimal numericGrade))
                    return numericGrade;
                return null; // Return null for non-numeric values like "INC"
            }
        }

        // Determine if the grade is below passing
        [NotMapped]
        public bool IsGradeLow => GradeAsNumber.HasValue && GradeAsNumber < 75;

        // Constructor to set CreatedAt
        public StudentSubject()
        {
            CreatedAt = DateTime.Now;
        }
    }
}
