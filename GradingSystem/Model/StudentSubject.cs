using GradingSystem.Model;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

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

    [Required]
    public string YearLevel { get; set; }
    [Required]
    public string Semester { get; set; }

    // Grade as a string to support "INC" or numeric values
    public string? Grade { get; set; } // Allow null values

    public DateTime CreatedAt { get; set; }

    [NotMapped]
    public string CourseCode => Subject?.CourseCode;

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

    public void SetYearLevelAndSemester(string yearLevel, string semester)
    {
        YearLevel = yearLevel;  // Now this works since we have a setter
        Semester = semester;    // Same here
    }

    // Constructor to set CreatedAt
    public StudentSubject()
    {
        CreatedAt = DateTime.Now;
    }
}