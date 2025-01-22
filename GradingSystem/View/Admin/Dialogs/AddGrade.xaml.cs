using GradingSystem.Data;
using GradingSystem.Model;
using GradingSystem.ViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Runtime.InteropServices.JavaScript;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GradingSystem.View.Admin.Dialogs
{
    public partial class AddGrade : Window
    {
        private readonly StudentSubjectViewModel _viewModel;
        private readonly SubjectViewModel _subject;
        private readonly StudentsViewModel _student;

        public AddGrade(ApplicationDbContext context)
        {
            InitializeComponent();
            _viewModel = new StudentSubjectViewModel(context);
            _subject = new SubjectViewModel(context);
            _student = new StudentsViewModel(context);
            DataContext = _viewModel;
            SubjectListDataGrid.DataContext = _subject;
        }

        // Handle dragging window by mouse
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        // Handle the Enter key event for loading subjects
        private async void StudentIdTxt_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                // Get the entered ID
                string studentId = idTxt.Text;

                // Find the student
                var student = _student.Students.FirstOrDefault(s => s.StudentId == studentId);

                if (student != null)
                {
                    // Update the UI with the student's information
                    studentNameTxt.Text = student.StudentName;
                    courseTxt.Text = student.ProgramId;
                    semesterTxt.Text = student.Semester;
                    yearLevelTxt.Text = student.YearLevel;
                    scholarshipTxt.Text = student.Status;

                    await _viewModel.LoadSubjects(studentId);
                }
                else
                {
                    MessageBox.Show("Student not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Column.Header.ToString() == "Grade")
            {
                // Get the edited row
                var studentSubject = e.Row.Item as StudentSubject;
                if (studentSubject != null)
                {
                    // Get the new grade value
                    var gradeCell = e.EditingElement as TextBox;
                    if (gradeCell != null)
                    {
                        string newGrade = gradeCell.Text;

                        // Additional validation for numeric grade and handle nullable decimal
                        decimal? parsedGrade = null;
                        if (decimal.TryParse(newGrade, out decimal grade))
                        {
                            parsedGrade = grade;
                        }
                        else
                        {
                            MessageBox.Show("Grade must be a valid numeric value.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        // Update the grade in the model
                        studentSubject.Grade = parsedGrade;

                        try
                        {
                            // Save changes to the database via ViewModel
                            await _viewModel.UpdateGradeAsync(studentSubject);
                            MessageBox.Show("Grade updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"An error occurred while saving the grade: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
        }

        private async void AddSubjectBtn(object sender, RoutedEventArgs e)
        {
            // Get the entered student ID
            string studentId = idTxt.Text;

            // Validate student ID
            if (string.IsNullOrEmpty(studentId))
            {
                MessageBox.Show("Please enter a valid Student ID.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Get the selected subject from the DataGrid
            var selectedSubject = SubjectListDataGrid.SelectedItem as Subject;
            if (selectedSubject == null)
            {
                MessageBox.Show("Please select a subject to add.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Check if the subject is already assigned to the student
                var existingSubject = _viewModel.StudentSubjects.FirstOrDefault(ss =>
                    ss.StudentId == studentId && ss.SubjectId == selectedSubject.SubjectId);

                if (existingSubject != null)
                {
                    MessageBox.Show("The selected subject is already assigned to the student.", "Duplicate Entry", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Create a new StudentSubject instance
                var newStudentSubject = new StudentSubject
                {
                    Id = $"{studentId}_{selectedSubject.SubjectId}", // Unique ID combining StudentId and SubjectId
                    StudentId = studentId,
                    SubjectId = selectedSubject.SubjectId,
                    Grade = null // Grade is initially null
                };

                // Add the subject using the ViewModel
                await _viewModel.AddStudentSubjectAsync(newStudentSubject);

                // Notify the user of success
                MessageBox.Show("Subject added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // Refresh the subject list for the student
                await _viewModel.LoadSubjects(studentId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while adding the subject: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void RemoveSubjectBtn(object sender, RoutedEventArgs e)
        {
            //var selectedSubject = SubjectListDataGrid.SelectedItem as StudentSubject;
            //if (selectedSubject != null)
            //{
            //    var confirmation = MessageBox.Show("Are you sure you want to remove this subject?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            //    if (confirmation == MessageBoxResult.Yes)
            //    {
            //        try
            //        {
            //            await _viewModel.RemoveStudentSubjectAsync(selectedSubject);
            //            MessageBox.Show("Subject removed successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            //            await _viewModel.LoadSubjects(idTxt.Text); // Refresh subjects
            //        }
            //        catch (Exception ex)
            //        {
            //            MessageBox.Show($"An error occurred while removing the subject: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //        }
            //    }
            //}
            //else
            //{
            //    MessageBox.Show("Please select a subject to remove.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            //}
        }
    }
}