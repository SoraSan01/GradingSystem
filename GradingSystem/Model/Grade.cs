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
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        public string StudentId { get; set; }

        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }

        [Required]
        [StringLength(50)]
        public string Program { get; set; }

        [Required]
        [StringLength(10)]
        public string YearLevel { get; set; }

        [Required]
        [StringLength(20)]
        public string Semester { get; set; }

        public DateTime CreatedAt { get; set; }

        // Updated StudentName property
        public string StudentName => $"{FirstName} {LastName}";

        public Grade()
        {
            CreatedAt = DateTime.Now;
        }
    }
}
