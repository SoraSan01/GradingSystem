using GradingSystem.Model;
using LiveCharts.Wpf;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GradingSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Program> Programs { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<GradeRequest> GradeRequests { get; set; }
        public DbSet<StudentSubject> StudentSubjects { get; set; }

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
                uniqueId = $"{studentId}-{subjectId}";
            }
            while (await context.StudentSubjects.AnyAsync(ss => ss.Id == uniqueId));

            return uniqueId;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();

                string connectionString = configuration.GetConnectionString("MyDbConnectionString");
                optionsBuilder.UseSqlServer(connectionString);
                optionsBuilder.EnableSensitiveDataLogging();

                optionsBuilder.UseLazyLoadingProxies();
            }

            base.OnConfiguring(optionsBuilder);
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StudentSubject>()
                .HasKey(ss => ss.Id);

            modelBuilder.Entity<Program>()
                .Property(p => p.ProgramId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany()
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
