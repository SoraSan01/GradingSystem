using GradingSystem.Data;
using GradingSystem.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GradingSystem.DataService
{
    public class DataService
    {
        private static DataService _instance;

        // Singleton pattern to ensure only one instance of DataService is used
        public static DataService Instance => _instance ??= new DataService();

        // Example data that will be preloaded (e.g., Subjects)
        public List<Subject> Subjects { get; set; }
        public List<Grade> Grades { get; set; }
        public List<StudentSubject> StudentSubjects { get; set; }
        public List<Student> Students { get; set; }
        public List<Program> Programs { get; set; }
        public List<User> Users { get; set; }

        public DataService()
        {
            Subjects = new List<Subject>();
        }
        public void LoadSubjects(ApplicationDbContext context)
        {
            // Load subjects only if they haven't been loaded yet
            if (Subjects.Count == 0)
            {
                Subjects = context.Subjects.ToList();
            }
        }
    }
}
