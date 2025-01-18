using GradingSystem.Data;
using GradingSystem.Model;
using GradingSystem.ViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GradingSystem.View.Admin.Dialogs
{
    public partial class AddGrade : Window
    {
        private readonly StudentSubjectViewModel _viewModel;

        public AddGrade(ApplicationDbContext context)
        {
            InitializeComponent();
            _viewModel = new StudentSubjectViewModel(context);
            DataContext = _viewModel;
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
                var studentId = idTxt.Text;

                if (!string.IsNullOrEmpty(studentId))
                {
                    try
                    {
                        MessageBox.Show($"Loading subjects for Student ID: {studentId}");
                        await _viewModel.LoadSubjects(studentId);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred while loading subjects: {ex.Message}",
                                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
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

                        // Validate the grade
                        if (string.IsNullOrEmpty(newGrade))
                        {
                            MessageBox.Show("Grade cannot be empty.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

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
    }
}