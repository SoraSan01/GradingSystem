using GradingSystem.Data;
using GradingSystem.Model;
using System;
using System.Linq;
using System.Windows;

namespace GradingSystem.ViewModel
{
    public class LoginViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public bool AuthenticateUser()
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    // Check if user exists in the database with the provided credentials
                    var user = context.Users.FirstOrDefault(u => u.Email == Email && u.Password == Password);

                    if (user != null)
                    {
                        // Login successful
                        MessageBox.Show($"Welcome, {user.Email}!", "Login Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                        return true;
                    }
                    else
                    {
                        // Login failed
                        MessageBox.Show("Invalid email or password.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}
