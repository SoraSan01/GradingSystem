using GradingSystem.Data;
using GradingSystem.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace GradingSystem.ViewModel
{
    public class StudentsViewModel : INotifyPropertyChanged
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

        // Method to load students from the database
        public void LoadStudents()
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    // Query the database to get all students
                    var studentList = context.Students.ToList();

                    // Clear the ObservableCollection and add the students
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

        // Method to add a new student
        public void AddStudent(Student newStudent)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    // Check if a student with the same name, email, or student ID already exists
                    var existingStudent = context.Students
                                                 .FirstOrDefault(s =>
                                                     (s.FirstName == newStudent.FirstName && s.LastName == newStudent.LastName) ||
                                                     (s.Email == newStudent.Email) ||
                                                     (s.StudentId == newStudent.StudentId));

                    if (existingStudent != null)
                    {
                        // Show message if student already exists and return immediately
                        MessageBox.Show("A student with the same name, email, or student ID already exists.", "Duplicate Student", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return; // Stop execution if duplicate is found
                    }

                    // If no duplicate, add the new student
                    context.Students.Add(newStudent);
                    context.SaveChanges();

                    // After saving, refresh the ObservableCollection and show success message
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Students.Add(newStudent);
                    });

                    MessageBox.Show("Student added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                // Show error message in case of any failure
                MessageBox.Show($"An error occurred while adding the student: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Method to delete a student
        public void DeleteStudent(Student student)
        {
            try
            {
                // Confirm deletion
                if (MessageBox.Show($"Are you sure you want to delete the student '{student.FirstName} {student.LastName}'?",
                    "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    using (var context = new ApplicationDbContext())
                    {
                        var studentToDelete = context.Students.Find(student.StudentId);

                        if (studentToDelete != null)
                        {
                            context.Students.Remove(studentToDelete);
                            context.SaveChanges();

                            // Remove the student from ObservableCollection
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                Students.Remove(student);
                            });

                            MessageBox.Show("Student deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Student not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while deleting the student: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Method to edit an existing student
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
                        // Check if another student with the same name, email, or student ID already exists (excluding the current student)
                        var existingStudent = context.Students
                            .FirstOrDefault(s =>
                                ((s.FirstName == updatedStudent.FirstName && s.LastName == updatedStudent.LastName) ||
                                s.Email == updatedStudent.Email || s.StudentId == updatedStudent.StudentId) &&
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
                        studentToUpdate.Program = updatedStudent.Program;
                        studentToUpdate.YearLevel = updatedStudent.YearLevel;

                        // Save changes to the database
                        context.SaveChanges();

                        // Reflect changes in the ObservableCollection (if necessary)
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            var studentInCollection = Students.FirstOrDefault(s => s.StudentId == updatedStudent.StudentId);
                            if (studentInCollection != null)
                            {
                                studentInCollection.FirstName = updatedStudent.FirstName;
                                studentInCollection.LastName = updatedStudent.LastName;
                                studentInCollection.Email = updatedStudent.Email;
                                studentInCollection.Program = updatedStudent.Program;
                                studentInCollection.YearLevel = updatedStudent.YearLevel;
                            }
                        });

                        MessageBox.Show("Student updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
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

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
