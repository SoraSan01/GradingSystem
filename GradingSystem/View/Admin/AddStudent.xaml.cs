using GradingSystem.Data;
using GradingSystem.Model;
using GradingSystem.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Threading.Tasks;
using Notifications.Wpf;

namespace GradingSystem.View.Admin
{
    public partial class AddStudent : Window
    {
        private readonly NotificationManager _notificationManager = new NotificationManager();

        public StudentsViewModel ViewModel { get; set; }
        public ProgramViewModel Program { get; set; }

        public event Action StudentAdded;

        private readonly ApplicationDbContext _context;

        public AddStudent(StudentsViewModel viewModel, ProgramViewModel programViewModel, ApplicationDbContext context)
        {
            InitializeComponent();
            ViewModel = viewModel;
            Program = programViewModel;
            _context = context ?? throw new ArgumentNullException(nameof(context));

            DataContext = ViewModel;

            _ = LoadProgramsAsync();
        }

        private async Task LoadProgramsAsync()
        {
            try
            {
                await Program.LoadProgramsAsync();
                programCmb.ItemsSource = Program.Programs;
            }
            catch (Exception ex)
            {
                ShowErrorNotification("Error loading programs", ex.Message);
            }
        }

        private async void AddStudentBtn(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields())
            {
                return;
            }

            try
            {
                var newStudent = CreateStudent();
                string year = yearCmb.SelectedValue?.ToString();
                string semester = semesterCmb.SelectedValue?.ToString();
                string programId = programCmb.SelectedValue?.ToString();
                string status = scholarCmb.SelectedValue?.ToString();

                if (string.IsNullOrWhiteSpace(programId))
                {
                    ShowErrorNotification("Validation Error", "Please select a valid program.");
                    return;
                }

                await ViewModel.AddStudentAsync(newStudent, year, semester, programId, status);

                StudentAdded?.Invoke();
                ClearForm();

                ShowSuccessNotification("Success", "Student added successfully!");
            }
            catch (Exception ex)
            {
                ShowErrorNotification("Error", $"An error occurred while adding the student: {ex.Message}");
            }
        }

        private bool ValidateFields()
        {
            if (string.IsNullOrWhiteSpace(FnameTxt.Text?.Trim()) ||
                string.IsNullOrWhiteSpace(LnameTxt.Text?.Trim()) ||
                string.IsNullOrWhiteSpace(idTxt.Text?.Trim()) ||
                string.IsNullOrWhiteSpace(emailTxt.Text?.Trim()) ||
                programCmb.SelectedItem == null || semesterCmb.SelectedItem == null ||
                yearCmb.SelectedItem == null)
            {
                MessageBox.Show("Please fill in all fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private Student CreateStudent()
        {
            return new Student
            {
                StudentId = idTxt.Text.Trim(),
                Email = emailTxt.Text.Trim(),
                FirstName = FnameTxt.Text.Trim(),
                LastName = LnameTxt.Text.Trim(),
                ProgramId = programCmb.SelectedValue?.ToString(),
                YearLevel = yearCmb.SelectedValue?.ToString(),
                Semester = semesterCmb.SelectedValue?.ToString(),
                Status = scholarCmb.SelectedValue?.ToString()
            };
        }

        private void ClearForm()
        {
            FnameTxt.Clear();
            LnameTxt.Clear();
            idTxt.Clear();
            emailTxt.Clear();
            programCmb.SelectedIndex = -1;
            yearCmb.SelectedIndex = -1;
            semesterCmb.SelectedIndex = -1;
            FnameTxt.Focus();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed && e.OriginalSource is not TextBox && e.OriginalSource is not Button)
            {
                this.DragMove();
            }
        }

        private void FnameTxt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            HandleTextInput(e, sender as TextBox, 20, char.IsLetter);
        }

        private void LnameTxt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            HandleTextInput(e, sender as TextBox, 20, char.IsLetter);
        }

        private void IdTxt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            HandleTextInput(e, sender as TextBox, 15, char.IsDigit);
        }

        private void emailTxt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            HandleTextInput(e, sender as TextBox, 50, _ => true);
        }

        private void HandleTextInput(TextCompositionEventArgs e, TextBox textBox, int maxLength, Func<char, bool> charValidation)
        {
            if (textBox != null && (textBox.Text.Length >= maxLength || !charValidation(e.Text[0])))
            {
                e.Handled = true;
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
                ShowInfoNotification("Goodbye", "Window closed successfully!");
                this.Close();
            }
        }

        private void CancelBtn(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ShowErrorNotification(string title, string message)
        {
            _notificationManager.Show(new NotificationContent
            {
                Title = title,
                Message = message,
                Type = NotificationType.Error
            });
        }

        private void ShowSuccessNotification(string title, string message)
        {
            _notificationManager.Show(new NotificationContent
            {
                Title = title,
                Message = message,
                Type = NotificationType.Success
            });
        }

        private void ShowInfoNotification(string title, string message)
        {
            _notificationManager.Show(new NotificationContent
            {
                Title = title,
                Message = message,
                Type = NotificationType.Information
            });
        }
    }
}
