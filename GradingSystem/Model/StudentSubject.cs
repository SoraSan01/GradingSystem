using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GradingSystem.Model
{
    public class StudentSubject
    {
        [Key]
        public String Id { get; set; }

        [Required]
        public string StudentId { get; set; }

        [ForeignKey(nameof(StudentId))]
        public Student Student { get; set; }

        [Required]
        public string SubjectId { get; set; }

        [ForeignKey(nameof(SubjectId))]
        public Subject Subject { get; set; }

        public DateTime CreatedAt { get; set; }

        public StudentSubject() { 
            CreatedAt = DateTime.Now;
        }
    }
}
