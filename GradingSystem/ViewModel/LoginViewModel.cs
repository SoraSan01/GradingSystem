using GradingSystem.Data;
using GradingSystem.Model;
using System;
using System.Linq;
using System.Windows;
using GradingSystem.DataService;
using Microsoft.EntityFrameworkCore; // Make sure this namespace is correct
using GradingSystem.DataService;

namespace GradingSystem.ViewModel
{
    public class LoginViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string UserRole { get; set; }  // Add UserRole property


        public async Task<bool> AuthenticateUser(ApplicationDbContext context)
        {
            try
            {
                // Check if user exists in the database with the provided credentials
                var user = await context.Users
                    .FirstOrDefaultAsync(u => (u.Email == Email || u.Username == Email) && u.Password == Password);

                if (user != null)
                {
                    // Set UserRole based on the role of the user
                    UserRole = user.Roles;  // Fix: Assign the Roles string directly to UserRole

                    // Login successful
                    MessageBox.Show($"Welcome, {user.Email}!", "Login Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                    return true;
                }
                else
                {
                    return false;
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
