using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GradingSystem.Model
{
    [Table("Program")]
    public class Program
    {
        [Key]
        public string ProgramId { get; set; }

        [Required]
        [StringLength(255)]
        public string ProgramName { get; set; }

        [Required]
        [StringLength(255)]
        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public Program()
        {
            CreatedAt = DateTime.Now;
        }
    }
}
