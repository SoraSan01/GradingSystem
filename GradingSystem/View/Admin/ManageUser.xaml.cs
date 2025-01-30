using GradingSystem.Data;
using GradingSystem.Model;
using GradingSystem.View.Admin.Dialogs;
using GradingSystem.ViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace GradingSystem.View.Admin
{
    /// <summary>
    /// Interaction logic for ManageUser.xaml
    /// </summary>
    public partial class ManageUser : UserControl
    {
        private readonly ApplicationDbContext _context;
        public UserViewModel User { get; set; }
        public ManageUser(ApplicationDbContext context)
        {
            InitializeComponent();
            _context = context;
            User = new UserViewModel(context);
            DataContext = User;
        }

        private void DeleteUserBtn(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var UserToDelete = button?.DataContext as User;

            if (UserToDelete != null)
            {
                var result = MessageBox.Show($"Are you sure you want to delete {UserToDelete.FirstName} {UserToDelete.LastName}?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _ = User.DeleteUserAsync(UserToDelete);
                }
            }
        }

        private void AddUserBtn(object sender, RoutedEventArgs e)
        {
            var AddUser = new AddUser(User);
            AddUser.ShowDialog();
        }

        private void EditButton(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var selectedUser = button?.DataContext as User;

            if (selectedUser != null)
            {
                var editUserWindow = new EditUser(User, selectedUser);
                editUserWindow.ShowDialog();

                // Refresh the user list after editing
                _ = User.LoadUsersAsync();
            }
            else
            {
                MessageBox.Show("Please select a user to edit.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            var filterText = textBox?.Text.ToLower();

            if (string.IsNullOrEmpty(filterText))
            {
                User.Users = new ObservableCollection<User>(await _context.Users.AsNoTracking().ToListAsync());
            }
            else
            {
                User.Users = new ObservableCollection<User>(await _context.Users
                    .AsNoTracking()
                    .Where(u => u.FirstName.ToLower().Contains(filterText) ||
                                u.LastName.ToLower().Contains(filterText) ||
                                u.Email.ToLower().Contains(filterText) ||
                                u.Username.ToLower().Contains(filterText) ||
                                u.Roles.ToLower().Contains(filterText) ||
                                u.UserId.ToLower().Contains(filterText))
                    .ToListAsync());
            }
        }
    }
}


