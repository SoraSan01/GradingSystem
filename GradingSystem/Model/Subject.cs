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
        [StringLength(100)]
        public string SubjectName { get; set; }

        [Required]
        [StringLength(50)]
        public string Schedule { get; set; }

        [Required]
        [StringLength(100)]
        public string ProfessorName { get; set; }

        public DateTime CreatedAt { get; set; }


        public Subject()
        {
            CreatedAt = DateTime.Now;
        }
    }
}
