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

        public event Action StudentAdded;

        private readonly ApplicationDbContext _context;

        public AddStudent(StudentsViewModel viewModel, ApplicationDbContext context)
        {
            InitializeComponent();
            ViewModel = viewModel;
            _context = context ?? throw new ArgumentNullException(nameof(context));

            DataContext = ViewModel;
        }

        private async void AddStudentBtn(object sender, RoutedEventArgs e)
        {
            string studentId = idTxt.Text.Trim();
            string firstName = FnameTxt.Text.Trim();
            string lastName = LnameTxt.Text.Trim();
            string email = emailTxt.Text.Trim();

            if (string.IsNullOrWhiteSpace(studentId) || string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(email))
            {
                ShowErrorNotification("Validation Error", "All fields are required.");
                return;
            }

            try
            {
                var student = new Student
                {
                    StudentId = studentId,
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email
                };

                await ViewModel.AddStudentAsync(student);

                ShowSuccessNotification("Success", "Student added successfully.");
                StudentAdded?.Invoke();
                Close();
            }
            catch (Exception ex)
            {
                ShowErrorNotification("Error", $"An error occurred while adding the student: {ex.Message}");
            }
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
