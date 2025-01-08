using GradingSystem.Data;
using GradingSystem.Model;
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
using GradingSystem.ViewModel;

namespace GradingSystem.View.Admin
{
    /// <summary>
    /// Interaction logic for AddUser.xaml
    /// </summary>
    public partial class AddUser : Window
    {

        public UserViewModel ViewModel { get; set; }

        public event Action UserAdded;

        public AddUser()
        {
            InitializeComponent();
        }

        private void window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed && e.OriginalSource is not TextBox && e.OriginalSource is not Button)
            {
                this.DragMove();
            }
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to exit?", "Close", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private void addUserBtn(object sender, RoutedEventArgs e)
        {
            if (ViewModel == null)
            {
                MessageBox.Show("ViewModel is not initialized.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(FnameTxt.Text?.Trim()) ||
                string.IsNullOrWhiteSpace(LnameTxt.Text?.Trim()) ||
                rolesCmb.SelectedItem == null || string.IsNullOrWhiteSpace(emailTxt.Text?.Trim()) ||
                string.IsNullOrWhiteSpace(passwordTxt.Text?.Trim()) || string.IsNullOrWhiteSpace(usernameTxt.Text?.Trim()) )
            {
                MessageBox.Show("Please fill in all fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var newUser = new User
                    {
                        UserId = context.GenerateUserId(),
                        FirstName = FnameTxt.Text.Trim(),
                        LastName = LnameTxt.Text.Trim(),
                        Email = emailTxt.Text.Trim(),
                        Password = passwordTxt.Text.Trim(),
                        Username = usernameTxt.Text.Trim(),
                        Roles = rolesCmb.SelectedValue?.ToString(),
                    };

                    ViewModel.AddUser(newUser);

                    UserAdded?.Invoke();
                    MessageBox.Show("User added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
    }
}
