using GradingSystem.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GradingSystem.Data
{
    class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Student> Students { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyDbConnectionString"].ConnectionString;
            optionsBuilder.UseSqlServer(connectionString);
        }

        public string GenerateStudentId()
        {
            string year = DateTime.Now.Year.ToString();

            // Get the highest existing ID for the current year
            string latestId = this.Students // Use 'this' to refer to the current context
                                   .Where(s => s.StudentId.StartsWith(year))
                                   .OrderByDescending(s => s.StudentId)
                                   .Select(s => s.StudentId)
                                   .FirstOrDefault();

            int nextNumber = 1; // Default to 1 if no records exist for the current year

            if (latestId != null)
            {
                // Extract the numeric part of the ID and increment it
                string numericPart = latestId.Substring(4);
                nextNumber = int.Parse(numericPart) + 1;
            }

            // Combine the year and the incremented number, padded to 4 digits
            return $"{year}{nextNumber:D4}";
        }

    }
}
