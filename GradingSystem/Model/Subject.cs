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
                throw new ArgumentException("Subject name cannot be null or empty.");
            }

            // Extract prefix from subject name
            string prefix = subjectName.Length >= 3
                ? subjectName.Substring(0, 3).ToUpper()
                : subjectName.ToUpper().PadRight(3, 'X'); // Pad with 'X' if subject name is too short

            if (existingIds == null || !existingIds.Any())
            {
                return $"{prefix}-001";
            }

            // Find the highest numeric suffix for the given prefix
            var matchingIds = existingIds
                .Where(id => id.StartsWith(prefix + "-"))
                .Select(id => id.Substring(prefix.Length + 1)) // Extract numeric part
                .Where(num => int.TryParse(num, out _)) // Ensure it's a number
                .Select(int.Parse);

            int nextNumber = matchingIds.Any() ? matchingIds.Max() + 1 : 1;

            return $"{prefix}-{nextNumber:000}";
        }
    }
}
