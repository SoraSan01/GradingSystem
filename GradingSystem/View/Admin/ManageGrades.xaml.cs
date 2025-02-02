using GradingSystem.Data;
using GradingSystem.Model;
using GradingSystem.View.Admin.Dialogs;
using GradingSystem.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Notifications.Wpf;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

        public ManageGrades(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            _viewModel = new StudentSubjectViewModel(serviceProvider);
            _subject = new SubjectViewModel(_context);
            _enrollment = serviceProvider.GetRequiredService<EnrollmentViewModel>();
            DataContext = _viewModel;
            SubjectListDataGrid.DataContext = _subject;
        }

        // Notification handling
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
            ShowNotification("Error", $"{action}: {ex.Message}", NotificationType.Error);
        }

        // Populate student details
        private void PopulateStudentDetails(Student student, Enrollment enrollment)
        {
            studentNameTxt.Text = enrollment.FullName;
            courseTxt.Text = enrollment.ProgramName;
            scholarshipTxt.Text = enrollment.Status;
        }

        // Validate grade input
        private bool IsValidGrade(string grade)
        {
            return !string.IsNullOrEmpty(grade) && (decimal.TryParse(grade, out _) || grade.Equals("INC", StringComparison.OrdinalIgnoreCase));
        }

        // Handle grade edit in DataGrid
        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Column.Header.ToString() == "Grade")
            {
                var studentSubject = e.Row.Item as StudentSubject;
                var gradeCell = e.EditingElement as TextBox;

                if (studentSubject != null && gradeCell != null)
                {
                    string newGrade = gradeCell.Text.Trim();
                    if (IsValidGrade(newGrade))
                    {
                        studentSubject.Grade = newGrade.Equals("INC", StringComparison.OrdinalIgnoreCase) ? "INC" : newGrade.ToUpper();
                        UpdateGradeAsync(studentSubject);
                    }
                    else
                    {
                        ShowNotification("Validation Error", "Grade must be a valid numeric value or 'INC'.", NotificationType.Warning);
                        gradeCell.Text = studentSubject.Grade ?? "INC";
                    }
                }
            }
        }

        // Update grade in the database
        private async Task UpdateGradeAsync(StudentSubject studentSubject)
        {
            try
            {
                await _viewModel.UpdateGradeAsync(studentSubject).ConfigureAwait(false);
                ShowNotification("Success", "Grade updated successfully.", NotificationType.Success);
            }
            catch (Exception ex)
            {
                HandleError("Updating grade", ex);
            }
        }

        // Handle grade input filtering
        private void GradePreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0) && !e.Text.ToUpper().Equals("I") && !e.Text.ToUpper().Equals("N") && !e.Text.ToUpper().Equals("C"))
            {
                e.Handled = true;
            }
        }

        // Remove subject from student's list
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

        // Load student data when Enter is pressed
        private async void StudentIdTxt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await LoadStudentDataAsync();
            }
        }

        // Load student data and subjects based on student ID
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
                var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == studentId).ConfigureAwait(false);

                if (student != null)
                {
                    var enrollment = await _context.Enrollments.FirstOrDefaultAsync(e => e.StudentId == studentId).ConfigureAwait(false);
                    if (enrollment != null)
                    {
                        await _viewModel.LoadSubjectsBasedOnStudentId(studentId);
                        Dispatcher.Invoke(() =>
                        {
                            StudentSubjectDatagrid.ItemsSource = _viewModel.StudentSubjects.Any() ? _viewModel.StudentSubjects : new List<StudentSubject>();
                            PopulateStudentDetails(student, enrollment);
                        });
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

        // Add subject to student
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

                var enrollments = await _context.Enrollments.Where(e => e.StudentId == studentId).ToListAsync();
                if (enrollments == null || !enrollments.Any())
                {
                    ShowNotification("Error", "No enrollments found for the student.", NotificationType.Error);
                    return;
                }

                bool isEligibleForSubjectAddition = enrollments.Any(e =>
                    e.Status == "Scholar" && e.YearLevel == "Fourth Year" && e.Semester == "Second Semester");

                if (!isEligibleForSubjectAddition)
                {
                    ShowNotification("Validation Error", "Only scholars in 4th year and 2nd semester can add subjects.", NotificationType.Warning);
                    return;
                }

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
            catch (Exception ex)
            {
                ShowNotification("Error", $"Error adding subject: {ex.Message}", NotificationType.Error);
            }
        }

        // Filter subjects based on year and semester selection
        private void FilterData()
        {
            string selectedYear = (YearComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string selectedSemester = (SemesterComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            var filteredSubjects = _viewModel.StudentSubjects
                .Where(subject =>
                    (string.IsNullOrEmpty(selectedYear) || subject.YearLevel.Equals(selectedYear, StringComparison.OrdinalIgnoreCase)) &&
                    (string.IsNullOrEmpty(selectedSemester) || subject.Semester.Equals(selectedSemester, StringComparison.OrdinalIgnoreCase))
                ).ToList();

            StudentSubjectDatagrid.ItemsSource = filteredSubjects;
        }

        // Event handlers for button clicks
        private void AddSubBtn(object sender, RoutedEventArgs e) => AddSubBtnAsync(sender, e);
        private void RemoveSubBtn(object sender, RoutedEventArgs e) => RemoveSubBtnAsync(sender, e);

        // Event handlers for combobox changes
        private void YearComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => FilterData();
        private void SemesterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => FilterData();
    }
}
