using GradingSystem.Model;
using GradingSystem.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GradingSystem.View.Admin.Dialogs
{
    /// <summary>
    /// Interaction logic for EditUser.xaml
    /// </summary>
    public partial class EditUser : Window
    {
        private readonly UserViewModel _userViewModel;

        public EditUser(UserViewModel userViewModel, User selectedUser)
        {
            InitializeComponent();
            _userViewModel = userViewModel ?? throw new ArgumentNullException(nameof(userViewModel));
            _userViewModel.SelectedUser = selectedUser ?? throw new ArgumentNullException(nameof(selectedUser));
            DataContext = _userViewModel;
            PasswordTxt.Password = selectedUser.Password; // Set the initial password
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed && e.OriginalSource is not TextBox && e.OriginalSource is not Button)
            {
                DragMove();
            }
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to exit?", "Close", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Close();
            }
        }

        private void CancelBtn(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void SaveBtn(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(UserIdTxt.Text) ||
                    string.IsNullOrWhiteSpace(FnameTxt.Text) ||
                    string.IsNullOrWhiteSpace(LnameTxt.Text) ||
                    string.IsNullOrWhiteSpace(EmailTxt.Text) ||
                    RoleCmb.SelectedValue == null ||
                    string.IsNullOrWhiteSpace(UsernameTxt.Text) ||
                    string.IsNullOrWhiteSpace(PasswordTxt.Password))
                {
                    MessageBox.Show("Please fill in all fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Update user object
                var updatedUser = _userViewModel.SelectedUser;
                updatedUser.UserId = UserIdTxt.Text.Trim();
                updatedUser.FirstName = FnameTxt.Text.Trim();
                updatedUser.LastName = LnameTxt.Text.Trim();
                updatedUser.Email = EmailTxt.Text.Trim();
                updatedUser.Username = UsernameTxt.Text.Trim();
                updatedUser.Password = PasswordTxt.Password.Trim();
                updatedUser.Roles = RoleCmb.SelectedValue?.ToString();

                // Save changes to the database
                await _userViewModel.EditUserAsync(updatedUser);

                // Notify success and close
                MessageBox.Show("User updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while saving the user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Minimize(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }
    }
}