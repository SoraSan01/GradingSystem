using GradingSystem.Data;
using GradingSystem.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GradingSystem.View.Admin
{
    /// <summary>
    /// Interaction logic for AddStudent.xaml
    /// </summary>
    public partial class AddStudent : Window
    {
        public AddStudent()
        {
            InitializeComponent();
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to exit?", "Close", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                this.Close();  // Close the application
            }
        }

        private void addStudentBtn(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FnameTxt.Text) || string.IsNullOrWhiteSpace(LnameTxt.Text)
                || courseCmb.SelectedItem == null || yearCmb.SelectedItem == null)
            {
                MessageBox.Show("Please fill in all fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                try
                {
                    using (var context = new ApplicationDbContext())  // Create an instance of your DbContext
                    {
                        // Create a new student object and populate it with the input data
                        var newStudent = new Student
                        {
                            StudentId = context.GenerateStudentId(), // Call GenerateStudentId using the context instance
                            FirstName = FnameTxt.Text,       // Assuming 'FnameTxt' is for the first name
                            LastName = LnameTxt.Text,        // Assuming 'LnameTxt' is for the last name
                            Course = courseCmb.SelectedItem.ToString(), // Assuming 'courseCmb' is the course combo box
                            YearLevel = yearCmb.SelectedItem.ToString(), // Assuming 'yearCmb' is the year combo box
                        };

                        // Add the new student to the database
                        context.Students.Add(newStudent);
                        context.SaveChanges();
                    }

                    // Show success message
                    MessageBox.Show("Student added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Optionally, clear the fields after adding the student
                    FnameTxt.Clear();
                    LnameTxt.Clear();
                    courseCmb.SelectedIndex = -1;
                    yearCmb.SelectedIndex = -1;
                }
                catch (Exception ex)
                {
                    // Handle any exceptions that may occur
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

    }
}
