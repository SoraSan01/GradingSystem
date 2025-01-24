using GradingSystem.Data;
using GradingSystem.Model;
using GradingSystem.View.Admin.Dialogs;
using GradingSystem.ViewModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

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
    }
}
