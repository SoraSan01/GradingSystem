using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GradingSystem.Model
{
    [Table("Subjects")]
    public class Subject
    {
        [Key]
        public string SubjectId { get; set; }

        [Required]
        [StringLength(50)]
        public string CourseCode { get; set; }

        [Required]
        [StringLength(255)]
        public string SubjectName { get; set; }

        public int Units { get; set; }

        // Foreign Key to Program
        [ForeignKey("Program")]
        public string ProgramId { get; set; }  // The ProgramId links to the Program

        public Program Program { get; set; }

        public string YearLevel { get; set; }

        public string Semester { get; set; }

        [StringLength(50)]
        public string? Schedule { get; set; }

        [StringLength(100)]
        public string? ProfessorName { get; set; }

        public DateTime CreatedAt { get; set; }

        public Subject()
        {
            CreatedAt = DateTime.Now;
        }

        public static string GenerateSubjectId(string subjectName, List<string> existingIds)
        {
            if (string.IsNullOrEmpty(subjectName))
            {
                throw new ArgumentException("Subject name cannot be null or empty");
            }

            // Extract the first three characters of the subject name
            string prefix = subjectName.Length >= 3 ? subjectName.Substring(0, 3).ToUpper() : subjectName.ToUpper();

            // Find the highest existing number for the given prefix
            var matchingIds = existingIds
                .Where(id => id.StartsWith(prefix + "-"))
                .Select(id => id.Substring(4)) // Extract numeric part
                .Select(num => int.TryParse(num, out int n) ? n : 0);

            int nextNumber = matchingIds.Any() ? matchingIds.Max() + 1 : 1;

            // Format the ID as "XXX-###"
            return $"{prefix}-{nextNumber:000}";
        }
    }
}
