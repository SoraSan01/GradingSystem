using GradingSystem.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GradingSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Program> Programs { get; set; }

        public DbSet<Grade> Grades { get; set; }

        public DbSet<Subject> Subjects { get; set; }
        public DbSet<GradeRequest> GradeRequests { get; set; }

        public string GenerateUserId()
        {
            string year = DateTime.Now.Year.ToString();

            // Fetch the latest ID from the database that matches the current year
            string latestId = null;

            try
            {
                using (var context = new ApplicationDbContext())
                {
                    latestId = context.Users
                                      .Where(u => u.UserId.StartsWith(year))
                                      .OrderByDescending(u => u.UserId)
                                      .Select(u => u.UserId)
                                      .FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                // Log the exception (adjust based on your logging framework)
                System.Diagnostics.Debug.WriteLine($"Error fetching latest UserId: {ex.Message}");
            }

            int nextNumber = 1;

            if (!string.IsNullOrEmpty(latestId))
            {
                // Safely parse the numeric part of the ID
                if (int.TryParse(latestId.Substring(4), out int numericPart))
                {
                    nextNumber = numericPart + 1;
                }
            }

            // Generate the new UserId with zero-padded number
            return $"{year}{nextNumber:D4}";
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyDbConnectionString"].ConnectionString;
            optionsBuilder.UseSqlServer(connectionString);

            base.OnConfiguring(optionsBuilder);
            optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
        }

    }
}
