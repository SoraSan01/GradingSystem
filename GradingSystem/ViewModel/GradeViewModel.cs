using GradingSystem.Data;
using GradingSystem.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GradingSystem.ViewModel
{
    public class GradeViewModel
    {
        public ObservableCollection<Grade> Grades { get; set; }

        public GradeViewModel()
        {
            // Initialize the ObservableCollection
            Grades = new ObservableCollection<Grade>();

            // Load data (this can be from your database or a static list for testing)
            LoadStudents();
        }

        public void LoadStudents()
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    // Query the database to get all students
                    var gradeList = context.Grades.ToList();

                    // Clear the ObservableCollection and add the students
                    Grades.Clear();
                    foreach (var student in gradeList)
                    {
                        Grades.Add(student);
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur
                System.Windows.MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }
    }
}
