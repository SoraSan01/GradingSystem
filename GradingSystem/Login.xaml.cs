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
using static MaterialDesignThemes.Wpf.Theme;

namespace GradingSystem
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private readonly LoginViewModel _viewModel;  // Declare _viewModel as a private field
        private ApplicationDbContext _context;

        public Login(): this(new ApplicationDbContext()) { }

        public Login(ApplicationDbContext context)
        {
            InitializeComponent();

            _context = context;

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
            loginFunc();
        }

        private void loginFunc()
        {
            // Retrieve user input
            _viewModel.Email = emailTxt.Text;
            _viewModel.Password = passwordTxt.Password;

            // Validate username or email
            if (string.IsNullOrWhiteSpace(_viewModel.Email))
            {
                MessageBox.Show("Please enter your email or username.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                clear();
                return;
            }

            // Check if input is an email format
            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            bool isEmail = System.Text.RegularExpressions.Regex.IsMatch(_viewModel.Email, emailPattern);

            if (isEmail)
            {
                // Additional email-specific validation (if needed)
            }
            else
            {
                // Validate username format (optional)
                if (_viewModel.Email.Contains(" ")) // Example: no spaces allowed in username
                {
                    MessageBox.Show("Username cannot contain spaces.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    clear();
                    return;
                }
            }

            // Validate password
            if (string.IsNullOrWhiteSpace(_viewModel.Password))
            {
                MessageBox.Show("Please enter your password.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                clear();
                return;
            }

            // If validation passes, attempt authentication
            if (_viewModel.AuthenticateUser())
            {
                // Open MainWindow upon successful login
                var mainWindow = new MainWindow(_context);
                mainWindow.Show();

                // Close the login window
                this.Close();
            }
            else
            {
                clear();
            }
        }

        private void clear()
        {
            emailTxt.Text = "";
            passwordTxt.Password = "";
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

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) { 
                loginFunc();
            }
        }
    }
}
