using GradingSystem.Data;
using GradingSystem.Model;
using GradingSystem.View.Admin.Dialogs;
using GradingSystem.ViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using Notifications.Wpf;
using System.Windows.Input;
using Microsoft.Extensions.Logging;

namespace GradingSystem.View.Admin
{
    public partial class ManageGrades : UserControl
    {
        private readonly NotificationManager _notificationManager = new NotificationManager();
        private readonly StudentSubjectViewModel _viewModel;
        private readonly SubjectViewModel _subject;
        private readonly EnrollmentViewModel _enrollment;
        private readonly ILogger<ManageGrades> _logger;
        private readonly ApplicationDbContext _context;

        public ManageGrades(ApplicationDbContext context)
        {
            InitializeComponent();
                _viewModel = new StudentSubjectViewModel(context);
                _subject = new SubjectViewModel(context);
                _enrollment = new EnrollmentViewModel(context);
                _context = context;
                DataContext = _viewModel;
                SubjectListDataGrid.DataContext = _subject;
        }

        private void ShowNotification(string title, string message, NotificationType type)
        {
            _notificationManager.Show(new NotificationContent
            {
                Title = title,
                Message = message,
                Type = type
            });
        }

        private void HandleError(string action, Exception ex)
        {
            _logger.LogError(ex, $"{action} failed");
            ShowNotification("Error", $"{action}: {ex.Message}", NotificationType.Error);
        }

        private async void StudentIdTxt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await LoadStudentDataAsync();
            }
        }

        private async Task LoadStudentDataAsync()
        {
            string studentId = idTxt.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(studentId))
            {
                ShowNotification("Validation Error", "Please enter a valid Student ID.", NotificationType.Warning);
                return;
            }

            try
            {
                var student = await _enrollment.GetStudentByIdAsync(studentId);
                if (student != null)
                {
                    var enrollment = await _context.Enrollments.FirstOrDefaultAsync(e => e.StudentId == studentId);
                    if (enrollment != null)
                    {
                        PopulateStudentDetails(student, enrollment);
                        await _viewModel.LoadSubjects(studentId);
                    }
                    else
                    {
                        ShowNotification("Error", "Enrollment not found.", NotificationType.Error);
                    }
                }
                else
                {
                    ShowNotification("Error", "Student not found.", NotificationType.Error);
                }
            }
            catch (Exception ex)
            {
                HandleError("Loading student data", ex);
            }
        }

        private void PopulateStudentDetails(Student student, Enrollment enrollment)
        {
            studentNameTxt.Text = student.StudentName;
            courseTxt.Text = enrollment.Program?.ProgramName ?? "N/A";
            semesterTxt.Text = enrollment.Semester;
            yearLevelTxt.Text = enrollment.YearLevel;
            scholarshipTxt.Text = enrollment.Status;
        }

        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Column.Header.ToString() == "Grade")
            {
                var studentSubject = e.Row.Item as StudentSubject;
                var gradeCell = e.EditingElement as TextBox;

                if (studentSubject != null && gradeCell != null)
                {
                    string newGrade = gradeCell.Text?.Trim().ToUpper();

                    if (IsValidGrade(newGrade))
                    {
                        studentSubject.Grade = newGrade.Equals("INC", StringComparison.OrdinalIgnoreCase) ? "INC" : newGrade;

                        Application.Current.Dispatcher.InvokeAsync(async () =>
                        {
                            try
                            {
                                await _viewModel.UpdateGradeAsync(studentSubject);
                                ShowNotification("Success", "Grade updated successfully.", NotificationType.Success);
                            }
                            catch (Exception ex)
                            {
                                HandleError("Updating grade", ex);
                            }
                        });
                    }
                    else
                    {
                        ShowNotification("Validation Error", "Grade must be a valid numeric value or 'INC'.", NotificationType.Warning);
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            gradeCell.Text = studentSubject.Grade ?? "INC";
                        });
                    }
                }
            }
        }

        private bool IsValidGrade(string grade)
        {
            return !string.IsNullOrEmpty(grade) && (decimal.TryParse(grade, out _) || grade.Equals("INC", StringComparison.OrdinalIgnoreCase));
        }

        private void GradePreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0) && e.Text.ToUpper() != "I" && e.Text.ToUpper() != "N" && e.Text.ToUpper() != "C")
            {
                e.Handled = true;
            }
        }

        private async void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = (sender as TextBox)?.Text?.Trim().ToLower() ?? string.Empty;
            string studentId = idTxt.Text?.Trim() ?? string.Empty;

            if (string.IsNullOrEmpty(studentId))
            {
                ShowNotification("Validation Error", "Please enter a valid Student ID.", NotificationType.Warning);
                return;
            }

            try
            {
                var student = await _enrollment.GetStudentByIdAsync(studentId);
                if (student == null)
                {
                    ShowNotification("Error", "Student not found.", NotificationType.Error);
                    return;
                }

                _subject.SearchText = searchText;
                _subject.ApplySearch();
            }
            catch (Exception ex)
            {
                HandleError("Searching subjects", ex);
            }
        }

        private async Task AddSubBtnAsync(object sender, RoutedEventArgs e)
        {
            string studentId = idTxt.Text?.Trim() ?? string.Empty;

            if (string.IsNullOrEmpty(studentId))
            {
                ShowNotification("Validation Error", "Please enter a valid Student ID.", NotificationType.Warning);
                return;
            }

            var selectedSubject = SubjectListDataGrid.SelectedItem as Subject;
            if (selectedSubject == null)
            {
                ShowNotification("Validation Error", "Please select a subject to add.", NotificationType.Warning);
                return;
            }

            try
            {
                var existingSubject = _viewModel.StudentSubjects.FirstOrDefault(ss =>
                    ss.StudentId == studentId && ss.SubjectId == selectedSubject.SubjectId);

                if (existingSubject != null)
                {
                    ShowNotification("Duplicate Entry", "The selected subject is already assigned to the student.", NotificationType.Information);
                    return;
                }

                var student = await _enrollment.GetStudentByIdAsync(studentId);
                if (student == null)
                {
                    ShowNotification("Error", "Student not found.", NotificationType.Error);
                    return;
                }

                var enrollment = await _context.Enrollments.FirstOrDefaultAsync(e => e.StudentId == studentId);
                if (enrollment == null)
                {
                    ShowNotification("Error", "Enrollment not found.", NotificationType.Error);
                    return;
                }

                if (enrollment.Status == "Scholar" && enrollment.YearLevel == "4th Year" && enrollment.Semester == "2nd Semester")
                {
                    var newStudentSubject = new StudentSubject
                    {
                        Id = $"{studentId}_{selectedSubject.SubjectId}",
                        StudentId = studentId,
                        SubjectId = selectedSubject.SubjectId,
                        Grade = null
                    };

                    await _viewModel.AddStudentSubjectAsync(newStudentSubject);
                    await _viewModel.LoadSubjects(studentId);

                    ShowNotification("Success", "Subject added successfully.", NotificationType.Success);
                }
                else
                {
                    ShowNotification("Validation Error", "Only scholars in 4th year and 2nd semester can add subjects.", NotificationType.Warning);
                }
            }
            catch (Exception ex)
            {
                HandleError("Adding subject", ex);
            }
        }

        private async Task RemoveSubBtnAsync(object sender, RoutedEventArgs e)
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
                        ShowNotification("Success", "Subject removed successfully.", NotificationType.Success);
                        await _viewModel.LoadSubjects(idTxt.Text ?? string.Empty);
                    }
                    catch (Exception ex)
                    {
                        HandleError("Removing subject", ex);
                    }
                }
            }
            else
            {
                ShowNotification("Validation Error", "Please select a subject to remove.", NotificationType.Warning);
            }
        }

        private async void AddSubBtn(object sender, RoutedEventArgs e)
        {
            await AddSubBtnAsync(sender, e);
        }

        private async void RemoveSubBtn(object sender, RoutedEventArgs e)
        {
            await RemoveSubBtnAsync(sender, e);
        }

        private void SearchTextBox(object sender, TextChangedEventArgs e)
        {
            // This method seems to be unused, consider removing it if not needed.
        }
    }
}
