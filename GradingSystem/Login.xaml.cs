using GradingSystem.Data;
using GradingSystem.ViewModel;
using System;
using GradingSystem;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GradingSystem
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private readonly LoginViewModel _viewModel;  // Declare _viewModel as a private field

        public Login()
        {
            InitializeComponent();

            // Initialize _viewModel with the ApplicationDbContext
            _viewModel = new LoginViewModel();

            // Set the DataContext for data binding
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
                // Call DragMove to allow the window to be dragged
                this.DragMove();
            }
        }

        private void loginBtn(object sender, RoutedEventArgs e)
        {
            // Retrieve user input
            _viewModel.Email = emailTxt.Text;
            _viewModel.Password = passwordTxt.Password;

            // Validate email
            if (string.IsNullOrWhiteSpace(_viewModel.Email))
            {
                MessageBox.Show("Please enter your email.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Check email format using regular expression
            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!System.Text.RegularExpressions.Regex.IsMatch(_viewModel.Email, emailPattern))
            {
                MessageBox.Show("Please enter a valid email address.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Validate password
            if (string.IsNullOrWhiteSpace(_viewModel.Password))
            {
                MessageBox.Show("Please enter your password.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_viewModel.Password.Length < 6) // Example: minimum length of 6 characters
            {
                MessageBox.Show("Password must be at least 6 characters long.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // If validation passes, attempt authentication
            if (_viewModel.AuthenticateUser())
            {
                // Open MainWindow upon successful login
                var mainWindow = new MainWindow();
                mainWindow.Show();

                // Close the login window
                this.Close();
            }
        }


        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
        }

        private void forgotBtn(object sender, RoutedEventArgs e)
        {
            ForgotPassword forgotPass = new ForgotPassword();
            forgotPass.Show();
            this.Close();
        }
    }
}
