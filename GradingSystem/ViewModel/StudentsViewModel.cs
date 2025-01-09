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
        public Student SelectedStudent { get; set; }

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
                    // Ensure this operation is done on the UI thread
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Students.Clear();
                        foreach (var student in studentList)
                        {
                            Students.Add(student);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur
                MessageBox.Show($"An error occurred while loading students: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        public void AddStudent(Student newStudent)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    // Check if a student with the same FirstName and LastName already exists
                    var existingStudent = context.Students
                                                 .FirstOrDefault(s => (s.FirstName == newStudent.FirstName && s.LastName == newStudent.LastName) ||
                                                 (s.Email == newStudent.Email) ||
                                                 (s.StudentId == newStudent.StudentId));

                    if (existingStudent != null)
                    {
                        // Show message if student already exists and return immediately
                        MessageBox.Show("A student with the same name already exists.", "Duplicate Student", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return; // Stop execution if duplicate is found
                    }

                    // If no duplicate, add the new student
                    context.Students.Add(newStudent);
                    context.SaveChanges();

                    // After saving, refresh the ObservableCollection and show success message
                    Students.Add(newStudent);

                    MessageBox.Show("Student added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                }
            }
            catch (Exception ex)
            {
                // Show error message in case of any failure
                MessageBox.Show($"An error occurred while adding the student: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

        public void EditStudent(Student updatedStudent)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    // Find the student by StudentId
                    var studentToUpdate = context.Students.Find(updatedStudent.StudentId);

                    if (studentToUpdate != null)
                    {
                        // Check if another student with the same name or email already exists (excluding the current student)
                        var existingStudent = context.Students
                            .FirstOrDefault(s =>
                                ((s.FirstName == updatedStudent.FirstName && s.LastName == updatedStudent.LastName) ||
                                s.Email == updatedStudent.Email) &&
                                s.StudentId != updatedStudent.StudentId); // Ensure StudentId is excluded

                        if (existingStudent != null)
                        {
                            // Show a message if a duplicate student is found
                            MessageBox.Show("A student with the same data already exists.", "Duplicate Student", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return; // Stop execution if duplicate is found
                        }

                        // If no duplicate, update the student properties
                        studentToUpdate.FirstName = updatedStudent.FirstName;
                        studentToUpdate.LastName = updatedStudent.LastName;
                        studentToUpdate.Email = updatedStudent.Email;
                        studentToUpdate.Course = updatedStudent.Course;
                        studentToUpdate.YearLevel = updatedStudent.YearLevel;

                        // Save changes to the database
                        context.SaveChanges();

                        // Optional: Reflect changes in the ObservableCollection or other relevant UI components
                        MessageBox.Show("Student saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }else
                    {
                        // If student is not found
                        MessageBox.Show("Student not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                // Show error message in case of any failure
                MessageBox.Show($"An error occurred while editing the student: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
