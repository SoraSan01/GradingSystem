using GradingSystem.Data;
using GradingSystem.Model;
using GradingSystem.ViewModel;
using Notifications.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GradingSystem.View.Admin.Dialogs
{
    public partial class AddSubject : Window
    {
        private readonly NotificationManager _notificationManager = new NotificationManager();
        public SubjectViewModel SubjectViewModel { get; set; }
        public ProgramViewModel Program { get; set; }

        public event Action SubjectAdded = delegate { };

        private readonly ApplicationDbContext _context;

        public AddSubject(ApplicationDbContext context, ProgramViewModel program)
        {
            InitializeComponent();
            _context = context;
            SubjectViewModel = new SubjectViewModel(context);
            Program = program;
            DataContext = SubjectViewModel;
            _ = LoadProgramsAsync(); // Await the method call
        }

        private async Task LoadProgramsAsync()
        {
            await Program.LoadProgramsAsync();
            ProgramCmb.ItemsSource = Program.Programs;
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
                this.Close();  // Simply close the window without triggering other actions
            }
        }

        private void CancelBtn(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

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

        private void ShowValidationError(string message)
        {
            MessageBox.Show(message, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private async void SaveBtn(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateForm()) return;

                var newSubject = new Subject
                {
                    SubjectId = ApplicationDbContext.GenerateSubjectId(CourseCodeTxt.Text.Trim(), new List<string>()),
                    CourseCode = CourseCodeTxt.Text.Trim(),
                    SubjectName = TitleTxt.Text.Trim(),
                    Units = int.Parse(UnitTxt.Text.Trim()),
                    ProgramId = ProgramCmb.SelectedValue?.ToString() ?? string.Empty,
                    YearLevel = yearCmb.SelectedValue?.ToString() ?? string.Empty,
                    Semester = SemesterCmb.SelectedValue?.ToString() ?? string.Empty,
                    Schedule = ScheduleTxt.Text.Trim(),
                    ProfessorName = ProfessorTxt.Text.Trim()
                };

                await SubjectViewModel.AddSubjectAsync(newSubject).ConfigureAwait(false);

                _notificationManager.Show(new NotificationContent
                {
                    Title = "Success",
                    Message = "Subject added successfully!",
                    Type = NotificationType.Success
                });

                // Only close the window after showing the success notification.
                this.Dispatcher.Invoke(() =>
                {
                    SubjectAdded?.Invoke();
                    this.Close();
                });
            }
            catch (Exception ex)
            {
                _notificationManager.Show(new NotificationContent
                {
                    Title = "Error",
                    Message = $"An error occurred: {ex.Message}",
                    Type = NotificationType.Error
                });
            }
        }

        private void BulkInsertBtn(object sender, RoutedEventArgs e)
        {
            var bulkInsert = new BulkInsertCourse(_context);
            bulkInsert.Show();
        }
    }
}
