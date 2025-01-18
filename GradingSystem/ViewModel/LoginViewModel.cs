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

        public async Task<bool> AuthenticateUser(ApplicationDbContext context)
        {
            try
            {
                    // Check if user exists in the database with the provided credentials
                    var user = await context.Users
                        .FirstOrDefaultAsync(u => (u.Email == Email || u.Username == Email) && u.Password == Password);

                    if (user != null)
                    {
                        // Login successful
                        MessageBox.Show($"Welcome, {user.Email}!", "Login Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                        PreloadData(context);
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

        // Preload data after successful login
        private void PreloadData(ApplicationDbContext context)
        {
            try
            {
                // Load subjects or other data you want to preload
                var subjects = context.Subjects.ToList();

                // Store the data in a shared service or memory so it can be accessed later
                DataService.DataService.Instance.Subjects = subjects;

                // Optionally, preload other data as needed (e.g., grades, assignments, etc.)
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while preloading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
