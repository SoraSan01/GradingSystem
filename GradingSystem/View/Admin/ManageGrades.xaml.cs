using GradingSystem.Data;
using GradingSystem.Model;
using GradingSystem.View.Admin.Dialogs;
using GradingSystem.ViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents.Serialization;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Xps.Packaging;
using System.Windows.Xps;

namespace GradingSystem.View.Admin
{
    public partial class ManageGrades : UserControl
    {
        private readonly ApplicationDbContext _context;
        public StudentsViewModel students { get; set; }

        public ManageGrades(ApplicationDbContext context)
        {
            InitializeComponent();

            _context = context;
            students = new StudentsViewModel(_context);
            DataContext = students;
        }

        private void AddGradeBtn(object sender, RoutedEventArgs e)
        {
            try
            {
                var addGradeDialog = new AddGrade(_context);
                addGradeDialog.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while opening the Add Grade dialog: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowGradeBtn(object sender, RoutedEventArgs e)
        {
            if (_context == null)
            {
                MessageBox.Show("Database context is not initialized.");
                return;
            }

            if (studentsDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Please select a student.");
                return;
            }


            try
            {
                // Check if a student is selected
                var selectedStudent = (Student)studentsDataGrid.SelectedItem;

                // Debugging: Verify if the student is selected
                if (selectedStudent == null)
                {
                    MessageBox.Show("No student selected.");
                    return;
                }

                // Debugging: Check if we reach this point
                MessageBox.Show("Student selected: " + selectedStudent.FirstName);  // Replace `Name` with the appropriate property

                // Ensure 'students' is properly initialized before using it
                if (students == null)
                {
                    MessageBox.Show("Students data is not initialized.");
                    return;
                }

                var showGradeWindow = new ShowGrade(selectedStudent, _context);
                showGradeWindow.DataContext = students;
                showGradeWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}\n{ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PrintDocuBtn(object sender, RoutedEventArgs e)
        {

        }      
    }
}