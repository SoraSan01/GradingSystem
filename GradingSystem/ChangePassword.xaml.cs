using GradingSystem.Data;
using System;
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
    /// Interaction logic for ChangePassword.xaml
    /// </summary>
    public partial class ChangePassword : Window
    {
        private readonly ApplicationDbContext _context;
        private string _email;

        public ChangePassword(string email)
        {
            InitializeComponent();
            _context = new ApplicationDbContext();
            _email = email; // Store the email passed from ForgotPasswordViewModel
        }

        private void submitBtn(object sender, RoutedEventArgs e)
        {
            string newPassword = passTxt.Password;
            string confirmPassword = confirmTxt.Password;

            // Check if both fields are filled in
            if (string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                MessageBox.Show("Both fields are required.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Check if the passwords match
            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Passwords do not match. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Password validation: Minimum 6 characters, at least one uppercase, one lowercase, one number, and one special character
            if (newPassword.Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters long.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var hasUpperCase = newPassword.Any(char.IsUpper);
            var hasLowerCase = newPassword.Any(char.IsLower);
            var hasDigit = newPassword.Any(char.IsDigit);
            var hasSpecialChar = newPassword.Any(ch => !char.IsLetterOrDigit(ch));

            if (!hasUpperCase || !hasLowerCase || !hasDigit || !hasSpecialChar)
            {
                MessageBox.Show("Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // Find the user in the database
                var user = _context.Users.FirstOrDefault(u => u.Email == _email);

                if (user == null)
                {
                    MessageBox.Show("User not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Update the user's password (ensure proper hashing in real-world applications)
                user.Password = newPassword;
                _context.SaveChanges();

                MessageBox.Show("Password successfully updated.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // Redirect to the login screen after successful password update
                Login login = new Login(_context);
                login.Show();
                this.Close(); // Close the ChangePassword window
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating password: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void closeBtn(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to exit?", "Close", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();  // Close the application
            }
        }
    }
}
