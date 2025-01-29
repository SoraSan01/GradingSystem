using GradingSystem.Data;
using GradingSystem.Model;
using GradingSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GradingSystem.View.Admin.Dialogs
{
    /// <summary>
    /// Interaction logic for AddSubject.xaml
    /// </summary>
    public partial class AddSubject : Window
    {
        public SubjectViewModel SubjectViewModel { get; set; }
        public ProgramViewModel Program { get; set; }

        // Renamed event for better context
        public event Action SubjectAdded;

        private readonly ApplicationDbContext _context;

        public AddSubject(ApplicationDbContext context, ProgramViewModel program)
        {
            InitializeComponent();
            _context = context;
            // Initialize SubjectViewModel with an empty Subject instance
            SubjectViewModel = new SubjectViewModel(context);
            ProgramCmb.ItemsSource = program.Programs;
            DataContext = SubjectViewModel;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed && e.OriginalSource is not TextBox && e.OriginalSource is not Button)
            {
                this.DragMove();
            }
        }

        private void Minimize(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to exit?", "Close", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private void CancelBtn(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Centralized Validation method
        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(CourseCodeTxt.Text))
            {
                ShowValidationError("Course Code cannot be empty.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(TitleTxt.Text))
            {
                ShowValidationError("Title cannot be empty.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(UnitTxt.Text) || !int.TryParse(UnitTxt.Text.Trim(), out _))
            {
                ShowValidationError("Units must be a valid integer.");
                return false;
            }
            if (ProgramCmb.SelectedValue == null)
            {
                ShowValidationError("Please select a Program.");
                return false;
            }
            if (yearCmb.SelectedValue == null)
            {
                ShowValidationError("Please select a Year Level.");
                return false;
            }
            if (SemesterCmb.SelectedValue == null)
            {
                ShowValidationError("Please select a Semester.");
                return false;
            }
            return true;
        }

        // Helper method to show validation errors
        private void ShowValidationError(string message)
        {
            MessageBox.Show(message, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private async void SaveBtn(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate form fields
                if (!ValidateForm()) return;

                var newSubject = new Subject
                {
                    SubjectId = ApplicationDbContext.GenerateSubjectId(CourseCodeTxt.Text.Trim(), new List<string>()),
                    CourseCode = CourseCodeTxt.Text.Trim(),
                    SubjectName = TitleTxt.Text.Trim(),
                    Units = int.Parse(UnitTxt.Text.Trim()),
                    ProgramId = ProgramCmb.SelectedValue?.ToString(),
                    YearLevel = yearCmb.SelectedValue?.ToString(),
                    Semester = SemesterCmb.SelectedValue?.ToString(),
                    Schedule = ScheduleTxt.Text.Trim(),
                    ProfessorName = ProfessorTxt.Text.Trim()
                };

                // Add the new subject
                await SubjectViewModel.AddSubjectAsync(newSubject);

                // Trigger event to notify that a subject was added
                SubjectAdded?.Invoke();

                // Close the window
                this.Close();
            }
            catch (Exception ex)
            {
                // Improved error message
                MessageBox.Show($"An error occurred while saving the subject: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BulkInsertBtn(object sender, RoutedEventArgs e)
        {
            var bulkInsert = new BulkInsertCourse(_context);
            bulkInsert.Show();
        }
    }
}
