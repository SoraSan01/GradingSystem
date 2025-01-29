using GradingSystem.Data;
using GradingSystem.Model;
using GradingSystem.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Linq;
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
            _student = new StudentsViewModel(context);
            _subject = new SubjectViewModel(context);
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
        private async void StudentIdTxt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string studentId = idTxt.Text?.Trim();
                if (string.IsNullOrEmpty(studentId))
                {
                    MessageBox.Show("Please enter a valid Student ID.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var student = _student.Students.FirstOrDefault(s => s.StudentId == studentId);
                if (student != null)
                {
                    // Update UI with student's details
                    studentNameTxt.Text = student.StudentName;
                    courseTxt.Text = student.Program?.ProgramName ?? "N/A";
                    semesterTxt.Text = student.Semester;
                    yearLevelTxt.Text = student.YearLevel;
                    scholarshipTxt.Text = student.Status;

                    // Load and display subjects
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
                var studentSubject = e.Row.Item as StudentSubject;
                var gradeCell = e.EditingElement as TextBox;

                if (studentSubject != null && gradeCell != null)
                {
                    string newGrade = gradeCell.Text?.Trim();

                    // Validate grade as a numeric value
                    if (decimal.TryParse(newGrade, out decimal parsedGrade))
                    {
                        studentSubject.Grade = parsedGrade;

                        try
                        {
                            // Update grade in database
                            await _viewModel.UpdateGradeAsync(studentSubject);
                            MessageBox.Show("Grade updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error updating grade: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Grade must be a valid numeric value.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private async void AddSubjectBtn(object sender, RoutedEventArgs e)
        {
            string studentId = idTxt.Text?.Trim();

            if (string.IsNullOrEmpty(studentId))
            {
                MessageBox.Show("Please enter a valid Student ID.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Ensure a subject is selected
            var selectedSubject = SubjectListDataGrid.SelectedItem as Subject;
            if (selectedSubject == null)
            {
                MessageBox.Show("Please select a subject to add.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Check if subject is already assigned
                var existingSubject = _viewModel.StudentSubjects.FirstOrDefault(ss =>
                    ss.StudentId == studentId && ss.SubjectId == selectedSubject.SubjectId);

                if (existingSubject != null)
                {
                    MessageBox.Show("The selected subject is already assigned to the student.", "Duplicate Entry", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var student = _student.Students.FirstOrDefault(s => s.StudentId == studentId);
                if (student == null)
                {
                    MessageBox.Show("Student not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Check if the student is a scholar, 4th year, and 2nd semester
                if (student.Status == "Scholar" && student.YearLevel == "4th Year" && student.Semester == "2nd Semester")
                {
                    var newStudentSubject = new StudentSubject
                    {
                        Id = $"{studentId}_{selectedSubject.SubjectId}", // Unique ID combining StudentId and SubjectId
                        StudentId = studentId,
                        SubjectId = selectedSubject.SubjectId,
                        Grade = null // Initially null
                    };

                    // Add subject using ViewModel
                    await _viewModel.AddStudentSubjectAsync(newStudentSubject);

                    // Notify success and refresh subject list
                    MessageBox.Show("Subject added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    await _viewModel.LoadSubjects(studentId); // Refresh subjects
                }
                else
                {
                    MessageBox.Show("Only scholars in 4th year and 2nd semester can add subjects.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding subject: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void RemoveSubjectBtn(object sender, RoutedEventArgs e)
        {
            var selectedSubject = StudentSubjectDatagrid.SelectedItem as StudentSubject;
            if (selectedSubject != null)
            {
                var confirmation = MessageBox.Show("Are you sure you want to remove this subject?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (confirmation == MessageBoxResult.Yes)
                {
                    try
                    {
                        await _viewModel.RemoveStudentSubjectAsync(selectedSubject);
                        MessageBox.Show("Subject removed successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        await _viewModel.LoadSubjects(idTxt.Text); // Refresh subjects
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error removing subject: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a subject to remove.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = (sender as TextBox)?.Text?.Trim().ToLower();
            string studentId = idTxt.Text?.Trim();

            if (string.IsNullOrEmpty(studentId))
            {
                MessageBox.Show("Please enter a valid Student ID.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var student = _student.Students.FirstOrDefault(s => s.StudentId == studentId);
            if (student == null)
            {
                MessageBox.Show("Student not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _subject.SearchText = searchText;
            _subject.ApplySearch();
        }
    }
}

