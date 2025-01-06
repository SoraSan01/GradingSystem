using GradingSystem.Data;
using GradingSystem.Model;
using GradingSystem.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GradingSystem.ViewModel
{
    public class StudentsViewModel
    {

        public ObservableCollection<Student> Students { get; set; }

        public StudentsViewModel()
        {
            // Initialize the ObservableCollection
            Students = new ObservableCollection<Student>();

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
                    var studentList = context.Students.ToList();

                    // Clear the ObservableCollection and add the students
                    Students.Clear();
                    foreach (var student in studentList)
                    {
                        Students.Add(student);
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur
                System.Windows.MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        public void AddStudent(Student newStudent)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    context.Students.Add(newStudent);
                    context.SaveChanges();

                    // Refresh the ObservableCollection
                    Students.Add(newStudent);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"An error occurred while adding the student: {ex.Message}");
            }
        }

        public void DeleteStudent(Student student)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var studentToDelete = context.Students.Find(student.StudentId);

                    if (studentToDelete != null)
                    {
                        context.Students.Remove(studentToDelete);
                        context.SaveChanges();

                        // Remove the student from ObservableCollection
                        Students.Remove(student);
                        MessageBox.Show("Student deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
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
