using GradingSystem.Data;
using GradingSystem.View.Encoder;
using GradingSystem.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Notifications.Wpf;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Notifications.Wpf;

namespace GradingSystem
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private readonly LoginViewModel _viewModel;  // Declare _viewModel as a private field
        private ApplicationDbContext _context;
        private readonly IServiceProvider _serviceProvider;
        private readonly NotificationManager _notificationManager = new NotificationManager();

        public Login(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;

            // Resolve ApplicationDbContext from the service provider
            _context = _serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Initialize ViewModel
            _viewModel = new LoginViewModel();
            DataContext = _viewModel;
        }


        private void closeBtn(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to exit?", "Close", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();  // Close the application
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove(); // Enable window dragging
            }
        }

        private void loginBtn(object sender, RoutedEventArgs e)
        {
            loginFunc();
        }

        private async void loginFunc()
        {
            _viewModel.Email = emailTxt.Text;
            _viewModel.Password = passwordTxt.Password;

            // Validate input fields
            if (!ValidateInputs())
                return;

            bool isAuthenticated = await _viewModel.AuthenticateUser(_context);

            if (isAuthenticated)
            {
                ShowNotification("Login Successful", "Welcome back!", NotificationType.Success);

                if (_viewModel.UserRole == "Admin")
                {
                    MainWindow mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
                    mainWindow.Show();
                }
                else if (_viewModel.UserRole == "Encoder" || _viewModel.UserRole == "Staff")
                {
                    EncoderMainWindow encoderWindow = _serviceProvider.GetRequiredService<EncoderMainWindow>();
                    encoderWindow.Show();
                }

                this.Close();
            }
            else
            {
                ShowNotification("Authentication Failed", "Invalid login credentials.", NotificationType.Error);
                clearFields();
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(_viewModel.Email))
            {
                ShowNotification("Validation Error", "Please enter your email or username.", NotificationType.Warning);
                return false;
            }

            if (IsValidEmail(_viewModel.Email))
            {
                // Email validation passed
            }
            else if (_viewModel.Email.Contains(" "))
            {
                ShowNotification("Validation Error", "Username cannot contain spaces.", NotificationType.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(_viewModel.Password))
            {
                ShowNotification("Validation Error", "Please enter your password.", NotificationType.Warning);
                return false;
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            // Regex to validate email format
            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailPattern);
        }

        private void clearFields()
        {
            emailTxt.Text = "";
            passwordTxt.Password = "";
        }

        private void forgotBtn(object sender, RoutedEventArgs e)
        {
            ForgotPassword forgotPass = new ForgotPassword(_context);
            forgotPass.Show();
            this.Close();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                loginFunc();
            }
        }

        // Optional: Toggle password visibility
        private void TogglePasswordVisibility(object sender, RoutedEventArgs e)
        {
            passwordTxt.Visibility = passwordTxt.Visibility == System.Windows.Visibility.Visible
                ? System.Windows.Visibility.Hidden
                : System.Windows.Visibility.Visible;
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
    }
}
