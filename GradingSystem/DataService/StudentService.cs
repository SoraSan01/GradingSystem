using GradingSystem.Data;
using GradingSystem.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GradingSystem.DataService
{
    public class StudentService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public StudentService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }
        public async Task<List<StudentSubject>> GetStudentSubjectsAsync(string studentId)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.StudentSubjects
                .Include(ss => ss.Subject)
                .Where(ss => ss.StudentId == studentId)
                .ToListAsync();
        }
    }
}
