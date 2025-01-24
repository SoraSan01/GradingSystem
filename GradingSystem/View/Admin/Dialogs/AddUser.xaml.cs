using GradingSystem.Data;
using GradingSystem.Model;
using GradingSystem.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GradingSystem.View.Admin.Dialogs
{
    /// <summary>
    /// Interaction logic for AddUser.xaml
    /// </summary>
    public partial class AddUser : Window
    {
        private readonly UserViewModel _userViewModel;

        public AddUser(UserViewModel userViewModel)
        {
            InitializeComponent();
            _userViewModel = userViewModel ?? throw new ArgumentNullException(nameof(userViewModel));
            DataContext = _userViewModel;
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

                // Create new user object
                User newUser = CreateNewUser();

                // Add user to the database and update DataGrid
                await _userViewModel.AddUserAsync(newUser);

                // Notify success and close
                MessageBox.Show("User added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while saving the user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private User CreateNewUser()
        {
            return new User
            {
                UserId = UserIdTxt.Text.Trim(),
                FirstName = FnameTxt.Text.Trim(),
                LastName = LnameTxt.Text.Trim(),
                Email = EmailTxt.Text.Trim(),
                Username = UsernameTxt.Text.Trim(),
                Password = PasswordTxt.Password.Trim(),
                Roles = RoleCmb.SelectedValue?.ToString(),
            };
        }

        private void Minimize(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }
    }
}
