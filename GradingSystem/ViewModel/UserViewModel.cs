using GradingSystem.Data;
using GradingSystem.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace GradingSystem.ViewModel
{
    public class UserViewModel : INotifyPropertyChanged
    {
        private User _selectedUser;
        private ObservableCollection<User> _users;
        private readonly ApplicationDbContext _context;

        public UserViewModel(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            Users = new ObservableCollection<User>();

            // Load users asynchronously during initialization
            _ = LoadUsersAsync();
        }

        public ObservableCollection<User> Users
        {
            get => _users;
            set
            {
                _users = value;
                OnPropertyChanged(nameof(Users));
            }
        }

        public User SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged(nameof(SelectedUser));
            }
        }

        /// <summary>
        /// Asynchronously loads users from the database and updates the ObservableCollection.
        /// </summary>
        public async Task LoadUsersAsync()
        {
            try
            {
                var userList = await Task.Run(() => _context.Users.ToList());

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

                // Add the new user to the ObservableCollection
                Application.Current.Dispatcher.Invoke(() => Users.Add(newUser));
            }
            catch (Exception ex)
            {
                await HandleErrorAsync("adding user", ex);
            }
        }

        public async Task EditUserAsync(User user)
        {
            var existingUser = await _context.Users.FindAsync(user.UserId);
            if (existingUser != null)
            {
                existingUser.FirstName = user.FirstName;
                existingUser.LastName = user.LastName;
                existingUser.Email = user.Email;
                existingUser.Username = user.Username;
                existingUser.Password = user.Password;
                existingUser.Roles = user.Roles;

                await _context.SaveChangesAsync();
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
                if (MessageBox.Show($"Are you sure you want to delete the user '{userToDelete.FullName}'?",
                                    "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    await Task.Run(() =>
                    {
                        _context.Users.Remove(userToDelete);
                        _context.SaveChanges();
                    });

                    // Remove the user from the ObservableCollection
                    Application.Current.Dispatcher.Invoke(() => Users.Remove(userToDelete));
                }
            }
            catch (Exception ex)
            {
                await HandleErrorAsync("deleting user", ex);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Centralized error handling for user-friendly error reporting.
        /// </summary>
        private async Task HandleErrorAsync(string action, Exception ex)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                MessageBox.Show($"An error occurred while {action}: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            });
        }
    }
}


