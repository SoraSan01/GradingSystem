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
    public class CourseViewModel
    {
        public ObservableCollection<Course> Courses { get; set; }

        public CourseViewModel()
        {
            // Initialize the ObservableCollection
            Courses = new ObservableCollection<Course>();

            // Load data (this can be from your database or a static list for testing)
            LoadCourse();
        }

        public void LoadCourse()
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    // Query the database to get all students
                    var courseList = context.Courses.ToList();

                    // Clear the ObservableCollection and add the students
                    Courses.Clear();
                    foreach (var Course in courseList)
                    {
                        Courses.Add(Course);
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
