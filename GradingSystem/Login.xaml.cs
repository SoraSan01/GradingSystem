using GradingSystem.Data;
using GradingSystem.View.Encoder;
using GradingSystem.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

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

            // Authenticate the user
            bool isAuthenticated = await _viewModel.AuthenticateUser(_context);  // Await the asynchronous method

            if (isAuthenticated)
            {
                // Show the main window based on the role
                if (_viewModel.UserRole == "Admin")
                {
                    MainWindow mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
                    mainWindow.Show();
                }
                else if (_viewModel.UserRole == "Encoder" || _viewModel.UserRole == "Staff")
                {
                    EncoderMainWindow encoderWindow = new EncoderMainWindow(_context);
                    encoderWindow.Show();
                }

                // Close the current (login) window
                this.Close();
            }
            else
            {
                // Handle invalid credentials
                MessageBox.Show("Invalid login credentials.", "Authentication Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                clearFields();
            }
        }

        private bool ValidateInputs()
        {
            // Validate email/username format
            if (string.IsNullOrWhiteSpace(_viewModel.Email))
            {
                MessageBox.Show("Please enter your email or username.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Validate email format
            if (IsValidEmail(_viewModel.Email))
            {
                // Additional validation logic for email (if required)
            }
            else
            {
                // Validate username (no spaces allowed)
                if (_viewModel.Email.Contains(" "))
                {
                    MessageBox.Show("Username cannot contain spaces.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }

            // Validate password
            if (string.IsNullOrWhiteSpace(_viewModel.Password))
            {
                MessageBox.Show("Please enter your password.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
    }
}
