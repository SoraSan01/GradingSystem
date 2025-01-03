﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GradingSystem.Model
{
    public class Student
    {
        [Key]
        public string StudentId { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [StringLength(50)]
        public string Course { get; set; }

        [Required]
        public string YearLevel { get; set; }
        public DateTime CreatedAt { get; set; }


        public Student()
        {
            CreatedAt = DateTime.Now;
        }


    }
}
