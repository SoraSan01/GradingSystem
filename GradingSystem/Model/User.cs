using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GradingSystem.Model
{
    public class User
    {
        [Key]
        public string UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 6)]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Roles { get; set; }

        public DateTime CreatedAt { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public User() {
            CreatedAt = DateTime.Now;

        }
    }
}
