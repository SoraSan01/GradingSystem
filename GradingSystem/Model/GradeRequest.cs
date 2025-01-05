using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GradingSystem.Model
{
    public class GradeRequest
    {
        [Key]
        public string RequestId { get; set; }

        [Required]
        public string StudentId { get; set; }

        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }

        public DateTime RequestDate { get; set; }

        [Required]
        [StringLength(100)]
        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }


        public GradeRequest()
        {
            CreatedAt = DateTime.Now;
            RequestDate = DateTime.Now;

        }
    }
}
