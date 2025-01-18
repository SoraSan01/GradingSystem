using GradingSystem.Model;
using LiveCharts.Wpf;
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

        public DbSet<StudentSubject> StudentSubjects { get; set; }

        public string GenerateUserId()
        {
            string year = DateTime.Now.Year.ToString();

            // Fetch the latest ID from the database that matches the current year using the injected context
            string latestId = this.Users
                                      .Where(u => u.UserId.StartsWith(year))
                                      .OrderByDescending(u => u.UserId)
                                      .Select(u => u.UserId)
                                      .FirstOrDefault();

            int nextNumber = 1;

            if (!string.IsNullOrEmpty(latestId))
            {
                if (int.TryParse(latestId.Substring(4), out int numericPart))
                {
                    nextNumber = numericPart + 1;
                }
            }

            return $"{year}{nextNumber:D4}";
        }

        public async Task<string> GenerateUniqueStudentSubjectId(ApplicationDbContext context, string studentId, string subjectId)
        {
            string uniqueId;
            do
            {
                uniqueId = $"{studentId}-{subjectId}-{DateTime.Now:yyyyMMddHHmmssfff}";
            }
            while (await context.StudentSubjects.AnyAsync(ss => ss.Id == uniqueId));

            return uniqueId;
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string connectionString = ConfigurationManager.ConnectionStrings["MyDbConnectionString"].ConnectionString;
                optionsBuilder.UseSqlServer(connectionString);
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StudentSubject>()
                .HasKey(ss => ss.Id);
            // Additional configurations if necessary
        }
    }
}
