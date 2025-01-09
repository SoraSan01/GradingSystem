using GradingSystem.Data;
using GradingSystem.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        public void AddCourse(Course newCourse)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    // Check if a student with the same FirstName and LastName already exists
                    var existingCourse = context.Courses
                                                 .FirstOrDefault(c => ((c.CourseName == newCourse.CourseName) ||
                                                 (c.CourseId == newCourse.CourseId) ||
                                                 (c.Description == newCourse.Description)));

                    if (existingCourse != null)
                    {
                        // Show message if student already exists and return immediately
                        MessageBox.Show("A Course with the same name already exists.", "Duplicate Student", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return; // Stop execution if duplicate is found
                    }

                    // If no duplicate, add the new student
                    context.Courses.Add(newCourse);
                    context.SaveChanges();

                    // After saving, refresh the ObservableCollection and show success message
                    Courses.Add(newCourse);

                    MessageBox.Show("Student added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                }
            }
            catch (Exception ex)
            {
                // Show error message in case of any failure
                MessageBox.Show($"An error occurred while adding the student: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void DeleteCourse(Course course)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var CourseToDelete = context.Courses.Find(course.CourseId);

                    if (CourseToDelete != null)
                    {
                        context.Courses.Remove(CourseToDelete);
                        context.SaveChanges();

                        Courses.Remove(course);
                        MessageBox.Show("Course deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while deleting the student: {ex.Message}");
            }
        }
    }
}
