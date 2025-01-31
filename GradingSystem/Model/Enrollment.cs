using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GradingSystem.Model
{
    public class Enrollment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EnrollmentID { get; set; }

        [Required]
        [ForeignKey("Student")]
        public string StudentId { get; set; }

        [Required]
        public string YearLevel { get; set; }

        [Required]
        public string Semester { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        [ForeignKey("Program")]
        public string ProgramId { get; set; }

        // Navigation properties
        public virtual Student Student { get; set; }
        public virtual Program Program { get; set; }
    }
}
