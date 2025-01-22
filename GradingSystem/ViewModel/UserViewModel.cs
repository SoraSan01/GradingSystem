using GradingSystem.Data;
using GradingSystem.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace GradingSystem.ViewModel
{
    public class UserViewModel
    {
        public ObservableCollection<User> Users { get; private set; }
        private readonly ApplicationDbContext _context;

        public UserViewModel(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            Users = new ObservableCollection<User>();

            // Load users asynchronously during initialization
            _ = LoadUsersAsync();
        }

        /// <summary>
        /// Asynchronously loads users from the database and updates the ObservableCollection.
        /// </summary>
        public async Task LoadUsersAsync()
        {
            try
            {
                var userList = await Task.Run(() => _context.Users.ToList());

                // Clear and add items to minimize UI updates
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Users.Clear();
                    foreach (var user in userList)
                    {
                        Users.Add(user);
                    }
                });
            }
            catch (Exception ex)
            {
                await HandleErrorAsync("loading users", ex);
            }
        }

        /// <summary>
        /// Adds a new user to the database and updates the ObservableCollection.
        /// </summary>
        public async Task AddUserAsync(User newUser)
        {
            if (newUser == null) throw new ArgumentNullException(nameof(newUser));

            try
            {
                await Task.Run(() =>
                {
                    _context.Users.Add(newUser);
                    _context.SaveChanges();
                });

                Application.Current.Dispatcher.Invoke(() => Users.Add(newUser));

                await ShowMessageAsync("User added successfully.", "Success", MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                await HandleErrorAsync("adding user", ex);
            }
        }

        /// <summary>
        /// Deletes a user from the database and updates the ObservableCollection.
        /// </summary>
        public async Task DeleteUserAsync(User userToDelete)
        {
            if (userToDelete == null) throw new ArgumentNullException(nameof(userToDelete));

            try
            {
                var confirmResult = MessageBox.Show(
                    $"Are you sure you want to delete the user '{userToDelete.FullName}'?",
                    "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (confirmResult == MessageBoxResult.Yes)
                {
                    await Task.Run(() =>
                    {
                        _context.Users.Remove(userToDelete);
                        _context.SaveChanges();
                    });

                    Application.Current.Dispatcher.Invoke(() => Users.Remove(userToDelete));

                    await ShowMessageAsync("User deleted successfully.", "Success", MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                await HandleErrorAsync("deleting user", ex);
            }
        }

        /// <summary>
        /// Displays a message box asynchronously.
        /// </summary>
        private async Task ShowMessageAsync(string message, string caption, MessageBoxImage icon)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                MessageBox.Show(message, caption, MessageBoxButton.OK, icon);
            });
        }

        /// <summary>
        /// Centralized error handling for user-friendly error reporting.
        /// </summary>
        private async Task HandleErrorAsync(string action, Exception ex)
        {
            await ShowMessageAsync($"An error occurred while {action}: {ex.Message}", "Error", MessageBoxImage.Error);
        }
    }
}
