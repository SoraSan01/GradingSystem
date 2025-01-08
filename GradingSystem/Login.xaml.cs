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

        private void loginFunc() {
            // Retrieve user input
            _viewModel.Email = emailTxt.Text;
            _viewModel.Password = passwordTxt.Password;

            // Validate email
            if (string.IsNullOrWhiteSpace(_viewModel.Email))
            {
                MessageBox.Show("Please enter your email.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                clear();
                return;
            }

            // Check email format using regular expression
            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!System.Text.RegularExpressions.Regex.IsMatch(_viewModel.Email, emailPattern))
            {
                MessageBox.Show("Please enter a valid email address.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                clear();
                return;
            }

            // Validate password
            if (string.IsNullOrWhiteSpace(_viewModel.Password))
            {
                MessageBox.Show("Please enter your password.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                clear();
                return;
            }

            if (_viewModel.Password.Length < 6) // Example: minimum length of 6 characters
            {
                MessageBox.Show("Password must be at least 6 characters long.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
